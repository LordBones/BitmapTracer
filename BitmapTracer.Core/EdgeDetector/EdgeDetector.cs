using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.EdgeDetector
{
    public class EdgeDetector
    {

        public static void EdgeReducer(CanvasEdgeVO dataCE, int threshold)
        {
            CanvasEdgeVO tmpCE = new CanvasEdgeVO(dataCE.Width, dataCE.Height);
            tmpCE.Clear();

            for (int y = 0; y < dataCE.Height; y++)
            {
                int diff = 0;

                for (int x = 0; x < dataCE.Width; x++)
                {
                    int index = y * dataCE.Width + x;

                    EdgePoint oneEdge = dataCE.Data[index];

                    if (oneEdge.Direction == GradientDirection.vertical || oneEdge.Direction == GradientDirection.askewRaise)
                    {
                        diff += oneEdge.Intensity;

                        if (diff > threshold)
                        {
                            oneEdge.Intensity = diff;
                            tmpCE.Data[index] = oneEdge;
                            diff = 0;
                        }
                    }
                }
            }

            for (int x = 0; x < dataCE.Width; x++)

            {
                int diff = 0;

                for (int y = 0; y < dataCE.Height; y++)
                {
                    int index = y * dataCE.Width + x;

                    EdgePoint oneEdge = dataCE.Data[index];

                    if (oneEdge.Direction == GradientDirection.horizontal || oneEdge.Direction == GradientDirection.askewFall)
                    {

                        diff += oneEdge.Intensity;

                        if (diff > threshold)
                        {
                            if (tmpCE.Data[index].Intensity < diff)
                            {
                                oneEdge.Intensity = diff;
                                tmpCE.Data[index] = oneEdge;
                            }
                            diff = 0;
                        }
                    }
                }
            }

            // prepiseme vysledek
            Array.Copy(tmpCE.Data, dataCE.Data, tmpCE.Data.Length);
        }
    }
}
