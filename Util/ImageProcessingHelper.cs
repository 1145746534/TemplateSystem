using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using TemplateSystem.Models;
using System.Runtime.InteropServices;

namespace TemplateSystem.Util
{
    internal class ImageProcessingHelper
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// 剪裁图像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="croppedImage"></param>
        /// <returns></returns>
        public static void Cropping(HObject image, out HObject croppedImage)
        {
            HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
            int x1 = 0;
            int y1 = 100;
            double w = width.D;
            int TargetWidth = (int)(w - y1 * 2);
            int TargetHeight = (int)height.D;
            HOperatorSet.CropPart(image, out croppedImage, x1, y1, TargetWidth, TargetHeight);
        }
        /// <summary>
        /// 彩色图转成灰度图 返回一个新对象
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static HObject RGBTransGray(HObject image)
        {
            //彩色图需转成灰度图
            HOperatorSet.CountChannels(image, out HTuple Channels);

            if (Channels.I == 3)
            {
                HOperatorSet.Decompose3(image, out HObject image1, out HObject image2, out HObject image3);
                image2.Dispose();
                image3.Dispose();
                Channels.Dispose();
                return image1.Clone();
            }
            else
                return image.Clone();

        }

        /// <summary>
        /// 灰度图转成彩色图
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static HObject GrayTransRGB(HObject image)
        {
            HObject ho_ImageRGB1 = null;
            HOperatorSet.GenEmptyObj(out ho_ImageRGB1);

            HOperatorSet.CopyImage(image, out HObject ho_DupImage1);

            HOperatorSet.CountChannels(ho_DupImage1, out HTuple hv_Channels);

            if ((int)(new HTuple(hv_Channels.TupleEqual(1))) != 0)
            {
               
                HOperatorSet.GetImagePointer1(ho_DupImage1, out HTuple hv_Pointer, out HTuple hv_Type, out HTuple hv_Width,
                    out HTuple hv_Height);
              
                HOperatorSet.GenImage3(out ho_ImageRGB1, hv_Type, hv_Width, hv_Height, hv_Pointer,
                    hv_Pointer, hv_Pointer);
                SafeHalconDispose(hv_Pointer);
                SafeHalconDispose(hv_Type);
                SafeHalconDispose(hv_Width);
                SafeHalconDispose(hv_Height);

            }

            SafeHalconDispose(hv_Channels);
            SafeDisposeHObject(ref ho_DupImage1);
            return ho_ImageRGB1;
        }

        public static void SafeHalconDispose<T>(T obj) where T : class, IDisposable
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = null; // 关键：解除引用使GC可回收
            }
        }

        /// <summary>
        /// 释放Halcon的HObject对象
        /// </summary>
        /// <param name="obj"></param>
        public static void SafeDisposeHObject(ref HObject obj)
        {
            if (obj != null && obj.IsInitialized())
            {
                obj.Dispose();
                obj = null;
            }
        }



        /// <summary>
        /// 处理 HTuple 的释放
        /// </summary>
        /// <param name="tuple"></param>
        public static void SafeDisposeHTuple(ref HTuple tuple)
        {
            if (tuple != null)
            {
                tuple.Dispose();
                tuple = null;
            }
        }

        // 安全克隆方法
        public static HObject CloneImageSafely(HObject source)
        {
            return (source != null && source.IsInitialized()) ? source.Clone() : null;
        }



        /// <summary>
        /// 定位轮毂
        /// </summary>
        /// <param name="image">源图像</param>
        /// <param name="minThreshold">最小阈值</param>
        /// <param name="maxThreshold">最大阈值</param>
        /// <returns>定位到的轮毂图像和轮毂轮廓</returns>
        public static PositioningWheelResultModel PositioningWheel(HObject imageSource, int minThreshold, int maxThreshold, int minRadius, bool isConfirmRadius = true)
        {
            PositioningWheelResultModel resultModel = new PositioningWheelResultModel();
            HObject image = null;
            try
            {
                image = CloneImageSafely(imageSource);
                //全图灰度
                resultModel.FullFigureGary = (float)GetIntensity(image);


                HOperatorSet.Threshold(image, out HObject region, minThreshold, maxThreshold);
                HOperatorSet.Connection(region, out HObject connectedRegions);
                HOperatorSet.FillUp(connectedRegions, out HObject regionFillUp);
                HOperatorSet.SelectShapeStd(regionFillUp, out HObject relectedRegions, "max_area", 70);
                HOperatorSet.InnerCircle(relectedRegions, out HTuple row, out HTuple column, out HTuple radius);


                SafeHalconDispose(region);
                SafeHalconDispose(connectedRegions);
                SafeHalconDispose(regionFillUp);
                SafeHalconDispose(relectedRegions);


                if (row.Length != 0) //存在轮毂
                {
                    //确认半径范围
                    if (isConfirmRadius && radius < minRadius)
                    {
                        //半径不符合
                    }
                    else
                    {
                        //获取内圆 通过找圆心的方式
                        HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                        HOperatorSet.CreateMetrologyModel(out HTuple MetrologyCircleHandle);
                        HOperatorSet.SetMetrologyModelImageSize(MetrologyCircleHandle, width, height);
                        HTuple H1 = new HTuple();
                        HTuple H2 = new HTuple();
                        HOperatorSet.AddMetrologyObjectCircleMeasure(MetrologyCircleHandle, row, column, radius + 10, 160, 3, 1, 30, H1, H2, out HTuple CircleIndex);
                        HOperatorSet.SetMetrologyObjectParam(MetrologyCircleHandle, CircleIndex, "num_instances", 1);
                        HOperatorSet.SetMetrologyObjectParam(MetrologyCircleHandle, CircleIndex, "min_score", 0.1);
                        HOperatorSet.ApplyMetrologyModel(image, MetrologyCircleHandle);

                        HOperatorSet.GetMetrologyObjectResult(MetrologyCircleHandle, CircleIndex, "all", "result_type", "all_param", out HTuple hv_Parameter);
                        HOperatorSet.GetMetrologyObjectMeasures(out HObject ho_Contours, MetrologyCircleHandle, CircleIndex, "all", out HTuple hv_Row, out HTuple hv_Column); //工具环
                        HOperatorSet.GetMetrologyObjectResultContour(out HObject ho_Contour, MetrologyCircleHandle, CircleIndex, "all", 1.5); //环
                        if (hv_Parameter.Length > 0)
                        {
                            row = hv_Parameter.TupleSelect(0);
                            column = hv_Parameter.TupleSelect(1);
                            radius = hv_Parameter.TupleSelect(2);
                        }

                        HOperatorSet.ClearMetrologyModel(MetrologyCircleHandle);
                        SafeHalconDispose(width);
                        SafeHalconDispose(height);
                        SafeHalconDispose(MetrologyCircleHandle);
                        SafeHalconDispose(H1);
                        SafeHalconDispose(H2);
                        SafeHalconDispose(CircleIndex);
                        SafeHalconDispose(hv_Parameter);
                        SafeHalconDispose(ho_Contours);
                        SafeHalconDispose(hv_Row);
                        SafeHalconDispose(hv_Column);
                        SafeHalconDispose(ho_Contour);



                        resultModel.CenterRow = row?.D;
                        resultModel.CenterColumn = column?.D;
                        resultModel.Radius = radius?.D;

                        HOperatorSet.GenCircle(out HObject reducedCircle, row, column, radius);
                        HOperatorSet.GenCircleContourXld(out HObject wheelContour, row, column, radius, 0, (new HTuple(360)).TupleRad(), "positive", 1.0);
                        HOperatorSet.ReduceDomain(image, reducedCircle, out HObject wheelImage);
                        HOperatorSet.Intensity(wheelImage, wheelImage, out HTuple mean, out HTuple deviation); //圈内灰度
                        resultModel.InnerCircleMean = (float)(mean.D);
                        resultModel.WheelImage = wheelImage.Clone();
                        resultModel.WheelContour = wheelContour.Clone();

                        SafeHalconDispose(reducedCircle);
                        SafeHalconDispose(wheelImage);
                        SafeHalconDispose(mean);
                        SafeHalconDispose(deviation);
                        SafeHalconDispose(wheelContour);


                    }
                }
                SafeHalconDispose(row);
                SafeHalconDispose(column);
                SafeHalconDispose(radius);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"PositioningWheelResultModel:{ex.Message}");
            }
            SafeHalconDispose(image);
            return resultModel;
        }

        /// <summary>
        /// 手动生成轮毂圈
        /// </summary>
        /// <param name="imageSource"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static PositioningWheelResultModel ManuslPositioningWheel(HObject imageSource, double row, double col, double radius)
        {
            PositioningWheelResultModel resultModel = new PositioningWheelResultModel();
            HObject image = null;
            try
            {
                image = CloneImageSafely(imageSource);
                //全图灰度
                resultModel.FullFigureGary = (float)GetIntensity(image);



                resultModel.CenterRow = row;
                resultModel.CenterColumn = col;
                resultModel.Radius = radius;

                HOperatorSet.GenCircle(out HObject reducedCircle, row, col, radius);
                HOperatorSet.GenCircleContourXld(out HObject wheelContour, row, col, radius, 0, (new HTuple(360)).TupleRad(), "positive", 1.0);
                HOperatorSet.ReduceDomain(image, reducedCircle, out HObject wheelImage);
                HOperatorSet.Intensity(wheelImage, wheelImage, out HTuple mean, out HTuple deviation); //圈内灰度
                resultModel.InnerCircleMean = (float)(mean.D);
                resultModel.WheelImage = wheelImage.Clone();
                resultModel.WheelContour = wheelContour.Clone();

                SafeHalconDispose(reducedCircle);
                SafeHalconDispose(wheelImage);
                SafeHalconDispose(mean);
                SafeHalconDispose(deviation);
                SafeHalconDispose(wheelContour);






            }
            catch (Exception ex)
            {
                Console.WriteLine($"PositioningWheelResultModel:{ex.Message}");
            }
            SafeHalconDispose(image);
            return resultModel;
        }




        /// <summary>
        /// 获取图片平均灰度值
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static double GetIntensity(HObject imageSource)
        {
            HObject grayImage = CloneImageSafely(imageSource);
            //全图灰度
            HOperatorSet.Intensity(grayImage, grayImage, out HTuple mean, out HTuple deviation);
            float fullFigureGary = (float)(mean.D);
            SafeHalconDispose(grayImage);
            SafeHalconDispose(mean);
            SafeHalconDispose(deviation);
            return fullFigureGary;
        }



        /// <summary>
        /// 轮毂识别算法
        /// </summary>
        /// <param name="image">源图像</param>
        /// <param name="templateDatas">模板数据</param>
        /// <param name="angleStart">起始角度</param>
        /// <param name="angleExtent">角度范围</param>
        /// <param name="minSimilarity">最小相似度</param>
        /// <returns>识别结果</returns>
        //public static RecognitionResultModel WheelRecognitionAlgorithm(HObject image, TemplateDatasModel templateDatas, double angleStart, double angleExtent, double minSimilarity)
        //{
        //    RecognitionResultModel activeIdentifyData = new RecognitionResultModel();
        //    RecognitionResultModel notActiveIdentifyData = new RecognitionResultModel();

        //    List<HTuple> rows = new List<HTuple>();
        //    List<HTuple> columns = new List<HTuple>();
        //    List<HTuple> angles = new List<HTuple>();

        //    List<HTuple> rowsN = new List<HTuple>();
        //    List<HTuple> columnsN = new List<HTuple>();
        //    List<HTuple> anglesN = new List<HTuple>();
        //    //活跃模板匹配，并将结果放入对应的匹配结果列表
        //    if (templateDatas.ActiveTemplates.Count > 0)
        //    {
        //        for (int i = 0; i < templateDatas.ActiveTemplates.Count; i++)
        //        {
        //            HOperatorSet.FindNccModel(image, templateDatas.ActiveTemplates[i], angleStart, angleExtent, 0.5, 1, 0.5, "true", 0,
        //                out HTuple row, out HTuple column, out HTuple angle, out HTuple score);

        //            activeIdentifyData.WheelTypes.Add(templateDatas.ActiveTemplateNames[i]);
        //            rows.Add(row);
        //            columns.Add(column);
        //            angles.Add(angle);

        //            if (score < 0.55)
        //                activeIdentifyData.Similaritys.Add(0.0);
        //            else
        //            {
        //                activeIdentifyData.Similaritys.Add(Math.Round(score.D, 3));
        //            }
        //        }
        //        //获取活跃模板匹配中的相似度最大值
        //        activeIdentifyData.Similarity = activeIdentifyData.Similaritys.Max();
        //    }
        //    else
        //        activeIdentifyData.Similarity = 0.0;

        //    //如果活跃模板匹配相似度最大值大于等于（系统设定识别成功的最小相似度 + 0.05 ），认为匹配成功 
        //    if (activeIdentifyData.Similarity >= minSimilarity + 0.05)
        //    {
        //        var index = activeIdentifyData.Similaritys.FindIndex(x => x == activeIdentifyData.Similarity);
        //        activeIdentifyData.RecognitionWheelType = activeIdentifyData.WheelTypes[index];
        //        activeIdentifyData.CenterRow = rows[index];
        //        activeIdentifyData.CenterColumn = columns[index];
        //        activeIdentifyData.Radian = angles[index];
        //        activeIdentifyData.TemplateID = templateDatas.ActiveTemplates[index];
        //        activeIdentifyData.IsInNotTemplate = false;
        //        return activeIdentifyData;
        //    }
        //    else
        //    {
        //        if (templateDatas.NotActiveTemplates.Count > 0)
        //        {
        //            for (int i = 0; i < templateDatas.NotActiveTemplates.Count; i++)
        //            {
        //                HOperatorSet.FindNccModel(image, templateDatas.NotActiveTemplates[i], angleStart, angleExtent, 0.5, 1, 0.5, "true", 0,
        //                    out HTuple row, out HTuple column, out HTuple angle, out HTuple score);

        //                notActiveIdentifyData.WheelTypes.Add(templateDatas.NotActiveTemplateNames[i]);
        //                activeIdentifyData.WheelTypes.Add(templateDatas.NotActiveTemplateNames[i]);
        //                rows.Add(row);
        //                columns.Add(column);
        //                angles.Add(angle);

        //                rowsN.Add(row);
        //                columnsN.Add(column);
        //                anglesN.Add(angle);

        //                if (score < 0.55)
        //                {
        //                    notActiveIdentifyData.Similaritys.Add(0.0);
        //                    activeIdentifyData.Similaritys.Add(0.0);
        //                }
        //                else
        //                {
        //                    notActiveIdentifyData.Similaritys.Add(Math.Round(score.D, 3));
        //                    activeIdentifyData.Similaritys.Add(Math.Round(score.D, 3));
        //                }
        //            }
        //            //获取不活跃模板匹配中的相似度最大值
        //            notActiveIdentifyData.Similarity = notActiveIdentifyData.Similaritys.Max();
        //            //如果活跃模板相似度最大值 大于等于 不活跃模板相似度最大值，且活跃模板相似度最大值 大于等于 设定值，则在活跃模板中识别成功
        //            if (activeIdentifyData.Similarity >= notActiveIdentifyData.Similarity && activeIdentifyData.Similarity >= minSimilarity)
        //            {
        //                var index = activeIdentifyData.Similaritys.FindIndex(x => x == activeIdentifyData.Similarity);
        //                activeIdentifyData.RecognitionWheelType = activeIdentifyData.WheelTypes[index];
        //                activeIdentifyData.CenterRow = rows[index];
        //                activeIdentifyData.CenterColumn = columns[index];
        //                activeIdentifyData.Radian = angles[index];
        //                activeIdentifyData.TemplateID = templateDatas.ActiveTemplates[index];
        //                activeIdentifyData.IsInNotTemplate = false;
        //                return activeIdentifyData;
        //            }
        //            //如果活跃模板相似度最大值 小于 不活跃模板相似度最大值，且不活跃模板相似度最大值 大于等于 设定值，则在不活跃模板中识别成功
        //            else if (activeIdentifyData.Similarity < notActiveIdentifyData.Similarity && notActiveIdentifyData.Similarity >= minSimilarity)
        //            {
        //                var index = notActiveIdentifyData.Similaritys.FindIndex(x => x == notActiveIdentifyData.Similarity);
        //                activeIdentifyData.RecognitionWheelType = notActiveIdentifyData.WheelTypes[index];
        //                activeIdentifyData.Similarity = notActiveIdentifyData.Similarity;
        //                activeIdentifyData.CenterRow = rowsN[index];
        //                activeIdentifyData.CenterColumn = columnsN[index];
        //                activeIdentifyData.Radian = anglesN[index];
        //                activeIdentifyData.TemplateID = templateDatas.NotActiveTemplates[index];
        //                activeIdentifyData.IsInNotTemplate = true;
        //                return activeIdentifyData;
        //            }
        //            //识别不成功
        //            else
        //            {
        //                activeIdentifyData.RecognitionWheelType = "NG";
        //                activeIdentifyData.Similarity = 0;
        //                activeIdentifyData.CenterRow = 0;
        //                activeIdentifyData.CenterColumn = 0;
        //                activeIdentifyData.Radian = 0;
        //                activeIdentifyData.TemplateID = null;
        //                activeIdentifyData.IsInNotTemplate = false;
        //                return activeIdentifyData;
        //            }
        //        }
        //        else
        //        {
        //            if (activeIdentifyData.Similarity >= minSimilarity)
        //            {
        //                var index = activeIdentifyData.Similaritys.FindIndex(x => x == activeIdentifyData.Similarity);
        //                activeIdentifyData.RecognitionWheelType = activeIdentifyData.WheelTypes[index];
        //                activeIdentifyData.CenterRow = rows[index];
        //                activeIdentifyData.CenterColumn = columns[index];
        //                activeIdentifyData.Radian = angles[index];
        //                activeIdentifyData.TemplateID = templateDatas.ActiveTemplates[index];
        //                activeIdentifyData.IsInNotTemplate = false;
        //                return activeIdentifyData;
        //            }
        //            else
        //            {
        //                activeIdentifyData.RecognitionWheelType = "NG";
        //                activeIdentifyData.Similarity = 0;
        //                activeIdentifyData.CenterRow = 0;
        //                activeIdentifyData.CenterColumn = 0;
        //                activeIdentifyData.Radian = 0;
        //                activeIdentifyData.TemplateID = null;
        //                activeIdentifyData.IsInNotTemplate = false;
        //                return activeIdentifyData;
        //            }
        //        }
        //    }
        //}

        public static RecognitionResultModel WheelRecognitionAlgorithm(HObject imageSource, List<TemplatedataModel> templateDatas, double angleStart,
                                                                        double angleExtent, double minSimilarity, out List<RecognitionResultModel> recognitionResults)
        {


            HObject image = CloneImageSafely(imageSource); //复制图片
            RecognitionResultModel result = null;
            recognitionResults = new List<RecognitionResultModel>();
            lock (_lock)
            {
                foreach (TemplatedataModel templateData in templateDatas)
                {
                    if (templateData.Template != null)
                    {

                        HOperatorSet.FindNccModel(image, templateData.Template,
                        angleStart, angleExtent, 0.5, 1, 0.5, "true", 0,
                        out HTuple row, out HTuple column, out HTuple angle, out HTuple score);


                        if (score != null && score.Length > 0 && score.D > 0.6)
                        {
                            RecognitionResultModel temp = new RecognitionResultModel();
                            temp.CenterRow = row.D;
                            temp.CenterColumn = column.D;
                            temp.Radian = angle.D;
                            temp.RecognitionWheelType = templateData.WheelType;
                            temp.Similarity = Math.Round(score.D, 3);
                            temp.WheelStyle = templateData.WheelStyle;
                            recognitionResults.Add(temp);
                            if (score.D > 0.85)
                            {
                                temp.status = "识别成功";
                                result = temp;
                                //templateData.UseTemplate();
                                break;
                            }

                        }
                        SafeHalconDispose(row);
                        SafeHalconDispose(column);
                        SafeHalconDispose(angle);
                        SafeHalconDispose(score);
                    }
                }
            }
            if (result == null) //没有直接匹配到 采用查询的方式
            {
                if (TryGetBestMatch(recognitionResults, minSimilarity + 0.05, out result))
                {
                    result.status = "识别成功";
                    //识别成功后 把这个轮形上次使用时间刷新
                    string _type = result.RecognitionWheelType;
                    var results = templateDatas
                        .Where(t => t.WheelType != null &&
                                    t.WheelType == _type);

                    // 输出结果
                    foreach (TemplatedataModel item in results)
                    {
                        foreach (var t in templateDatas)
                        {
                            //t.UseTemplate();
                        }
                    }
                }
                else
                {
                    //没有找到合格品
                    result = new RecognitionResultModel
                    {
                        RecognitionWheelType = "NG"
                    };
                }
            }
            SafeHalconDispose(image);
            return result;



            //HObject image = CloneImageSafely(imageSource);
            //RecognitionResultModel resultIfFailed = new RecognitionResultModel
            //{
            //    RecognitionWheelType = "NG"
            //};

            //foreach (var templateData in templateDatas.Where(t => t.Use == true))
            //{
            //    if (templateData.Template != null)
            //    {
            //        HOperatorSet.FindNccModel(
            //        image, templateData.Template,
            //        angleStart, angleExtent,
            //        0.5, 1, 0.5,
            //        "true", 0,
            //        out HTuple row, out HTuple column, out HTuple angle, out HTuple score);

            //        if (score != null && score.Length > 0 && score.D > 0.6)
            //        {

            //            recognitionResults.Add(new RecognitionResultModel
            //            {
            //                CenterRow = row.D,
            //                CenterColumn = column.D,
            //                Radian = angle.D,
            //                RecognitionWheelType = templateData.WheelType,
            //                Similarity = Math.Round(score.D, 3),
            //                WheelStyle = templateData.WheelStyle
            //            });
            //        }
            //        SafeHalconDispose(row);
            //        SafeHalconDispose(column);
            //        SafeHalconDispose(angle);
            //        SafeHalconDispose(score);
            //    }


            //}

            //// 如果成功，尝试找最高相似度且满足阈值的
            //if (TryGetBestMatch(recognitionResults, minSimilarity + 0.05, out RecognitionResultModel bestMatch))
            //{
            //    bestMatch.status = "识别成功";
            //    //识别成功后 把这个轮形上次使用时间刷新
            //    string _type = bestMatch.RecognitionWheelType;
            //    var results = templateDatas
            //        .Where(t => t.WheelType != null &&
            //                    t.WheelType == _type)
            //        .ToList();

            //    // 输出结果
            //    foreach (var item in results)
            //    {
            //        item.LastUsedTime = DateTime.Now;
            //    }
            //    return bestMatch;
            //}

            //// 活跃模板未匹配到，尝试非活跃模板
            ////performMatching(false);
            //foreach (var templateData in templateDatas.Where(t => t.Use == false))
            //{
            //    if (templateData.Template != null)
            //    {

            //        HOperatorSet.FindNccModel(image, templateData.Template, angleStart, angleExtent,
            //                0.5, 1, 0.5, "true", 0, out HTuple row, out HTuple column, out HTuple angle,
            //                out HTuple score);

            //        if (score != null && score.Length > 0 && score.D > 0.6)
            //        {

            //            recognitionResults.Add(new RecognitionResultModel
            //            {
            //                CenterRow = row.D,
            //                CenterColumn = column.D,
            //                Radian = angle.D,
            //                RecognitionWheelType = templateData.WheelType,
            //                Similarity = Math.Round(score.D, 3),
            //                WheelStyle = templateData.WheelStyle
            //            });
            //        }
            //        SafeHalconDispose(row);
            //        SafeHalconDispose(column);
            //        SafeHalconDispose(angle);
            //        SafeHalconDispose(score);


            //    }


            //}


            //if (TryGetBestMatch(recognitionResults, minSimilarity + 0.05, out bestMatch))
            //{
            //    bestMatch.status = "识别成功";
            //    //识别成功后 把这个轮形上次使用时间刷新
            //    string _type = bestMatch.RecognitionWheelType;
            //    List<TemplatedataModels> results = templateDatas
            //        .Where(t => t.WheelType != null &&
            //                    t.WheelType == _type)
            //        .ToList();

            //    // 输出结果
            //    foreach (TemplatedataModels item in results)
            //    {
            //        item.LastUsedTime = DateTime.Now;
            //    }
            //    return bestMatch;
            //}
            //SafeHalconDispose(image);
            //return resultIfFailed;
        }


        public static RecognitionResultModel WheelRecognitionAlgorithm1(HObject imageSource, List<TemplatedataModel> templateDatas, double angleStart,
                                                                        double angleExtent, double minSimilarity, out List<RecognitionResultModel> recognitionResults)
        {


            HObject image = CloneImageSafely(imageSource); //复制图片
            float fullGray = (float)GetIntensity(image);
            RecognitionResultModel result = null;
            recognitionResults = new List<RecognitionResultModel>();
            lock (_lock)
            {
                foreach (TemplatedataModel templateData in templateDatas)
                {
                    if (templateData.Template != null)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetGenericShapeModelParam(templateData.Template, "angle_start", (new HTuple(-180)).TupleRad()
                                );
                        }
                        HOperatorSet.SetGenericShapeModelParam(templateData.Template, "border_shape_models", "false");

                        HTuple hv_MatchResultID = new HTuple();
                        HTuple hv_NumMatchResult = new HTuple();
                        HOperatorSet.FindGenericShapeModel(image, templateData.Template, out hv_MatchResultID,
                                                            out hv_NumMatchResult);
                        // 检查是否找到匹配结果
                        if (hv_NumMatchResult.I != 0)
                        {
                            // 获取所有匹配结果的分数
                            List<double> scores = new List<double>();
                            for (int i = 0; i < hv_NumMatchResult.I; i++)
                            {
                                HTuple score = new HTuple();
                                HOperatorSet.GetGenericShapeModelResult(
                                    hv_MatchResultID,
                                    i,
                                    "score",
                                    out score
                                );
                                scores.Add(score.D);
                            }
                            // 找到最高分数的索引
                            double maxValue = scores.Max();
                            int maxIndex = scores.FindIndex(x => x == maxValue);

                            HOperatorSet.GetGenericShapeModelResultObject(out HObject ho_MatchContour, hv_MatchResultID,
                                                                            maxIndex, "contours");

                            //get_generic_shape_model_result(MatchResultID, I, 'hom_mat_2d', HomMat2D)
                            //hom_mat2d_identity(AlignmentHomMat2D)
                            //hom_mat2d_translate(AlignmentHomMat2D, -RefRow, -RefColumn, AlignmentHomMat2D)
                            //hom_mat2d_compose(HomMat2D, AlignmentHomMat2D, AlignmentHomMat2D)
                            

                           

                            RecognitionResultModel temp = new RecognitionResultModel();
                            temp.RecognitionWheelType = templateData.WheelType;
                            temp.Similarity = Math.Round(maxValue, 3);
                            temp.WheelStyle = templateData.WheelStyle;
                            temp.FullFigureGary = fullGray;
                            temp.RecognitionContour = ho_MatchContour?.Clone();
                            temp.CenterRow = templateData.PositionCircleRow;
                            temp.CenterColumn = templateData.PositionCircleColumn;
                            temp.Radius = templateData.CircumCircleRadius;
                            if (templateData.TemplateAreaCenterRow != 0
                                && templateData.TemplateAreaCenterColumn != 0)
                            {
                                HOperatorSet.GetGenericShapeModelResult(hv_MatchResultID, maxIndex, "hom_mat_2d", out HTuple HomMat2D);
                                HOperatorSet.HomMat2dIdentity(out HTuple AlignmentHomMat2D);
                                HOperatorSet.HomMat2dTranslate(AlignmentHomMat2D, -templateData.TemplateAreaCenterRow,
                                    -templateData.TemplateAreaCenterColumn, out AlignmentHomMat2D);
                                HOperatorSet.HomMat2dCompose(HomMat2D, AlignmentHomMat2D, out AlignmentHomMat2D);
                                //Console.WriteLine($"变换矩阵1: {AlignmentHomMat2D.ToString()}");
                                //Console.WriteLine($"变换矩阵2: {HomMat2D.ToString()}");
                                temp.HomMat2D = AlignmentHomMat2D?.Clone();
                                SafeDisposeHTuple(ref HomMat2D);
                            }
                           
                            recognitionResults.Add(temp);
                            SafeHalconDispose(ho_MatchContour);
                           
                            //if (maxValue > 0.85)
                            //{
                            //    temp.status = "识别成功";
                            //    result = temp;
                            //    break;
                            //}

                        }

                        SafeHalconDispose(hv_MatchResultID);
                        SafeHalconDispose(hv_NumMatchResult);

                    }
                }
            }
            if (result == null) //没有直接匹配到 采用查询的方式
            {
                if (TryGetBestMatch(recognitionResults, minSimilarity + 0.05, out result))
                {
                    result.status = "识别成功";

                    //-----验证----
                    TemplatedataModel target = templateDatas.FirstOrDefault(t => t.WheelType == result.RecognitionWheelType);
                    HObject ho_ModelContours;
                   
                    HOperatorSet.GetShapeModelContours(out ho_ModelContours, target.Template, 1);
                    //// 计算模板轮廓的中心
                    HObject ho_ModelRegion;
                    HOperatorSet.GenRegionContourXld(ho_ModelContours, out ho_ModelRegion, "filled");
                    HTuple hv_AreaModel, hv_RowModel, hv_ColModel;
                    HOperatorSet.AreaCenter(ho_ModelRegion, out hv_AreaModel, out hv_RowModel, out hv_ColModel);

                    // 计算变换后轮廓的中心
                    HObject ho_TransRegion;
                    HOperatorSet.GenRegionContourXld(result.RecognitionContour, out ho_TransRegion, "filled");
                    HTuple hv_AreaTrans, hv_RowTrans, hv_ColTrans;
                    HOperatorSet.AreaCenter(ho_TransRegion, out hv_AreaTrans, out hv_RowTrans, out hv_ColTrans);

                    // 计算平移量

                    HTuple hv_RefRow1 = hv_RowTrans[hv_RowTrans.Length - 1].D - hv_RowModel[hv_RowModel.Length - 1].D;
                    HTuple hv_RefColumn1 = hv_ColTrans[hv_ColTrans.Length - 1].D - hv_ColModel[hv_ColModel.Length - 1].D;
                    Console.WriteLine($"实际参数 - hv_RefRow1:{target.TemplateAreaCenterRow} hv_RefColumn1:{target.TemplateAreaCenterColumn}");
                    Console.WriteLine($"验证参数 - hv_RefRow2:{hv_RefRow1} hv_RefColumn2:{hv_RefColumn1}");

                    if(target.TemplateAreaCenterRow == 0)
                    {
                        target.TemplateAreaCenterRow =(float) hv_RefRow1.D;
                    }
                    if (target.TemplateAreaCenterColumn == 0)
                    {
                        target.TemplateAreaCenterColumn = (float)hv_RefColumn1.D;
                    }
                    //-----验证-----


                }
                else
                {
                    //没有找到合格品
                    result = new RecognitionResultModel
                    {
                        RecognitionWheelType = "NG"
                    };
                }
            }
            SafeHalconDispose(image);
            return result;

        }

        // 辅助方法：从识别结果中找出最高相似度的对象
        private static bool TryGetBestMatch(
            List<RecognitionResultModel> results,
            double similarityThreshold,
            out RecognitionResultModel bestMatch)
        {
            if (results.Count == 0)
            {

                bestMatch = new RecognitionResultModel
                {
                    RecognitionWheelType = "NG"
                };
                return false;
            }

            bestMatch = results.OrderByDescending(r => r.Similarity).First();

            return bestMatch.Similarity > similarityThreshold;
        }

        //public static RecognitionResultModel WheelRecognitionAlgorithm(HObject image, List<TemplatedataModels> templateDatas, double angleStart, double angleExtent, double minSimilarity, List<RecognitionResultModel> list = null)
        //{
        //    List<RecognitionResultModel> Recognition = new List<RecognitionResultModel>();
        //    RecognitionResultModel IdentifyData = new RecognitionResultModel();
        //    IdentifyData.status = "识别失败";
        //    IdentifyData.RecognitionWheelType = "NG";
        //    //活跃模板匹配，并将结果放入对应的匹配结果列表
        //    foreach (TemplatedataModels templateData in templateDatas)
        //    {
        //        if (templateData.Use)
        //        {
        //            HOperatorSet.FindNccModel(image, templateData.Template, angleStart, angleExtent, 0.5, 1, 0.5, "true", 0,
        //               out HTuple row, out HTuple column, out HTuple angle, out HTuple score);
        //            //查找记录保存起来 - 需要更新最后匹配时间
        //            if (score != null && score > 0.5)
        //            {
        //                RecognitionResultModel item = new RecognitionResultModel();
        //                item.CenterRow = row;
        //                item.CenterColumn = column;
        //                item.Radian = angle;
        //                item.RecognitionWheelType = templateData.TemplateName;
        //                item.Similarity = Math.Round(score.D, 3);
        //                Recognition.Add(item);
        //            }
        //        }
        //    }
        //    //活跃模板匹配完成
        //    if (Recognition.Count > 0)
        //    {
        //        double maxSim = Recognition.Max(x => x.Similarity);
        //        if (maxSim > minSimilarity + 0.05) //找到最大匹配相似度
        //        {
        //            IdentifyData = Recognition.Find(a => a.Similarity > maxSim);
        //            return IdentifyData;
        //        }
        //    }


        //    //活跃模板中未匹配到所需要的轮形
        //    foreach (TemplatedataModels templateData in templateDatas)
        //    {
        //        if (!templateData.Use)
        //        {

        //            HOperatorSet.FindNccModel(image, templateData.Template, angleStart, angleExtent, 0.5, 1, 0.5, "true", 0,
        //               out HTuple row, out HTuple column, out HTuple angle, out HTuple score);
        //            //查找记录保存起来 - 需要更新最后匹配时间
        //            if (score != null && score > 0.5)
        //            {
        //                RecognitionResultModel item = new RecognitionResultModel();
        //                item.CenterRow = row;
        //                item.CenterColumn = column;
        //                item.Radian = angle;
        //                item.RecognitionWheelType = templateData.TemplateName;
        //                item.Similarity = Math.Round(score.D, 3);
        //                Recognition.Add(item);
        //            }

        //        }
        //    }

        //    if (Recognition.Count > 0)
        //    {
        //        double maxSim = Recognition.Max(x => x.Similarity);
        //        if (maxSim > minSimilarity + 0.05) //找到最大匹配相似度
        //        {
        //            IdentifyData = Recognition.Find(a => a.Similarity > maxSim);
        //            return IdentifyData;
        //        }
        //    }
        //    return IdentifyData;



        //}


        /// <summary>
        /// 获取仿射后的模板轮廓
        /// </summary>
        /// <param name="modelID">模板ID</param>
        /// <param name="newRow">新位置的行坐标</param>
        /// <param name="newColumn">新位置的列坐标</param>
        /// <param name="newRadian">新位置的角度</param>
        /// <param name="templateContour">仿射后的模板轮廓</param>
        public static HObject GetAffineTemplateContour(HTuple modelID, HTuple newRow, HTuple newColumn, HTuple newRadian)
        {
            HTuple model = modelID.Clone();
            HOperatorSet.GetNccModelRegion(out HObject modelRegion, model);
            HOperatorSet.VectorAngleToRigid(0, 0, 0, newRow, newColumn, newRadian, out HTuple homMat2D);
            HOperatorSet.AffineTransRegion(modelRegion, out HObject regionAffineTrans, homMat2D, "nearest_neighbor");
            HOperatorSet.GenContourRegionXld(regionAffineTrans, out HObject templateContour, "border_holes");

            SafeHalconDispose(modelRegion);
            SafeHalconDispose(model);
            SafeHalconDispose(newRow);
            SafeHalconDispose(newColumn);
            SafeHalconDispose(newRadian);
            SafeHalconDispose(homMat2D);
            SafeHalconDispose(regionAffineTrans);

            return templateContour;
        }

        public static async Task<string> SaveImageDatasAsync(HObject _image, SaveWay way, string ImagePath, string prefixName = null)
        {
            HObject saveImage = CloneImageSafely(_image);

            string savePath = string.Empty;
            DateTime dateTime = DateTime.Now;

            // 路径定义
            string handImagesPath = string.Empty;
            string monthPath = string.Empty;
            string dayPath = string.Empty;
            string ngPath = string.Empty;

            try
            {
                if (way == SaveWay.Hand)
                {
                    handImagesPath = ImagePath;
                    Directory.CreateDirectory(handImagesPath);

                }
                else
                {
                    monthPath = Path.Combine(ImagePath, $"{dateTime.Month}月");
                    dayPath = Path.Combine(monthPath, $"{dateTime.Day}日");
                    ngPath = Path.Combine(dayPath, "NG");
                    Directory.CreateDirectory(monthPath); // CreateDirectory 自动处理已存在的情况
                    Directory.CreateDirectory(dayPath);
                    Directory.CreateDirectory(ngPath);
                }

                // 异步获取磁盘空间
                double diskFree = await Task.Run(() => GetHardDiskFreeSpace("D"));

                if (diskFree <= 200)
                {
                    // UI 线程安全的消息显示
                    //Application.Current.Dispatcher.Invoke(() =>
                    //    EventMessage.MessageDisplay("磁盘存储空间不足，请检查！", true, false));
                    return savePath;
                }

                // 根据保存方式构建路径
                string saveWheelTypePath = "";
                if (way == SaveWay.AutoOK)
                {
                    // 查找下划线的位置
                    int index = prefixName.IndexOf("_", StringComparison.Ordinal);

                    // 如果找到双下划线，返回前面的部分
                    if (index >= 0)
                    {
                        string value = prefixName.Substring(0, index);
                        string finallyName = prefixName.Contains("半") ? "半" : "成";
                        value = $"{value}_{finallyName}";
                        saveWheelTypePath = Path.Combine(dayPath, value);

                    }
                    else
                        saveWheelTypePath = Path.Combine(dayPath, prefixName);

                    await Task.Run(() => Directory.CreateDirectory(saveWheelTypePath));
                    savePath = Path.Combine(saveWheelTypePath, $"{prefixName}&{dateTime:yyMMddHHmmss}.tif");
                }
                else if (way == SaveWay.AutoNG)
                {
                    savePath = Path.Combine(ngPath, $"NG&{dateTime:yyMMddHHmmss}.tif");
                }
                else
                {
                    savePath = Path.Combine(handImagesPath, $"Hand&{dateTime:yyMMddHHmmss}.tif");
                }
                string saveImagePath = string.Empty;
                // 异步保存图像
                await Task.Run(() =>
                {
                    saveImagePath = savePath.Replace(@"\", "/");
                    HOperatorSet.WriteImage(saveImage, "tiff", 0, saveImagePath);
                    SafeHalconDispose(saveImage);
                });
                //if (way == SaveWay.Hand)
                //    Application.Current.Dispatcher.Invoke(() =>
                //        EventMessage.MessageDisplay($"图片保存成功：{saveImagePath}", true, false));

                return savePath;
            }
            catch (Exception ex)
            {
                // 异常处理（可根据需要记录日志）
                //Application.Current.Dispatcher.Invoke(() =>
                //    EventMessage.MessageDisplay($"保存失败: {ex.Message}", true, false));
                return string.Empty;
            }
        }

        public enum SaveWay
        {
            [Description("自动OK图")]
            AutoOK,
            [Description("自动NG图")]
            AutoNG,
            [Description("手动")]
            Hand
        }

        ///  <summary> 
        /// 获取指定驱动器的剩余空间总大小(单位为MB) 
        ///  </summary> 
        ///  <param name="HardDiskName">代表驱动器的字母(必须大写字母) </param> 
        ///  <returns> </returns> 
        private static long GetHardDiskFreeSpace(string HardDiskName)
        {
            long freeSpace = new long();
            HardDiskName = HardDiskName + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace / (1024 * 1024);
                }
            }
            return freeSpace;
        }



        /// <summary>
        /// 获取线性渐变画刷
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static LinearGradientBrush GetLinearGradientBrush(Color color)//Colors.red
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.StartPoint = new Point(0, 0);
            linearGradientBrush.EndPoint = new Point(0, 1);
            linearGradientBrush.GradientStops.Add(new GradientStop(color, 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1.2));
            return linearGradientBrush;
        }
    }

}
