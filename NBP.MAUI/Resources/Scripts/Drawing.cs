namespace NBP.MAUI.Resources.Scripts
{
    internal class Drawing : IDrawable
    {
        public List<double> dniKursu = new();
        public string jakiKurs = "";
        private int offset = 50;
        private float height;
        private float width;
        private float diagramWidth = 0;

        public Drawing(float h, float w)
        {
            height = h;
            width = w;
        }

        public void Draw(ICanvas canvas, RectF rect)
        {
            float diagramHeight = 200;
            diagramWidth = width - 2 * offset;

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 5;
            canvas.FontSize = 30;

            if (dniKursu.Count == 0)
            {
                canvas.DrawLine(offset, height - offset / 2, offset, height - offset - diagramHeight);
                canvas.DrawLine(offset, height - offset / 2, width - offset, height - offset / 2);

                return;
            }

            canvas.DrawString(jakiKurs, width / 2, offset / 2, HorizontalAlignment.Center);

            canvas.DrawLine(offset, height - offset / 2, offset, height - offset - diagramHeight);
            canvas.DrawLine(offset, height - offset / 2, width - offset, height - offset / 2);

            double max = dniKursu.Max();
            float heightSpacing = diagramHeight / (float)max;
            float widthSpacing = diagramWidth / 30;

            canvas.FontSize = 15;
            canvas.DrawString(max.ToString("0.00"), offset, height - offset - diagramHeight, HorizontalAlignment.Right);
            canvas.DrawString("0.00", offset, height - offset / 2, HorizontalAlignment.Right);

            for(int i = 0; i < 30; i++)
            {
                canvas.DrawCircle(new PointF(offset + (widthSpacing * (i + 1)),(height - offset / 2) - (float)(heightSpacing * dniKursu[i])), 3);
            }
        }
    }
}
