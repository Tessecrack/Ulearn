using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using FractalPainting.Infrastructure.Common;
using FractalPainting.Infrastructure.UiActions;

namespace FractalPainting.App
{
    public class ImageSettingsAction : IUiAction
    {
        private readonly IImageHolder imageHolderContainer;
        private readonly ImageSettings imageSettingsContainer;

        public ImageSettingsAction(IImageHolder imageHolder, ImageSettings imageSettings)
        {
            this.imageHolderContainer = imageHolder;
            this.imageSettingsContainer = imageSettings;
        }

        public MenuCategory Category => MenuCategory.Settings;
        public string Name => "Изображение...";
        public string Description => "Размеры изображения";

        public void Perform()
        {
            SettingsForm.For(imageSettingsContainer).ShowDialog();
            imageHolderContainer.RecreateImage(imageSettingsContainer);
        }
    }

    public class SaveImageAction : IUiAction
    {
        private readonly IImageHolder imageHolder;
        private readonly AppSettings appSettings;
        public SaveImageAction(IImageHolder imageHolder, AppSettings appSettings)
        {
            this.imageHolder = imageHolder;
            this.appSettings = appSettings;
        }
        public MenuCategory Category => MenuCategory.File;
        public string Name => "Сохранить...";
        public string Description => "Сохранить изображение в файл";

        public void Perform()
        {
            var dialog = new SaveFileDialog
            {
                CheckFileExists = false,
                InitialDirectory = Path.GetFullPath(appSettings.ImagesDirectory),
                DefaultExt = "bmp",
                FileName = "image.bmp",
                Filter = "Изображения (*.bmp)|*.bmp"
            };
            var res = dialog.ShowDialog();
            if (res == DialogResult.OK)
                imageHolder.SaveImage(dialog.FileName);
        }
    }

    public class PaletteSettingsAction : IUiAction
    {
        private readonly Palette paletteContainer;
        public PaletteSettingsAction(Palette palette)
        {
            paletteContainer = palette;
        }
        public MenuCategory Category => MenuCategory.Settings;
        public string Name => "Палитра...";
        public string Description => "Цвета для рисования фракталов";
        public void Perform()
        {
            SettingsForm.For(paletteContainer).ShowDialog();
        }
    }

    public class MainForm : Form
    {
        public MainForm()
        {

        }
        /*public MainForm()
            : this(
                new IUiAction[]
                {
                    new SaveImageAction(),
                    new DragonFractalAction(),
                    new KochFractalAction(),
                    new ImageSettingsAction(),
                    new PaletteSettingsAction()
                }, Services.GetPictureBoxImageHolder())
        { }*/

        public MainForm(IUiAction[] actions, PictureBoxImageHolder pictureBox)
        {
            var imageSettings = CreateSettingsManager().Load().ImageSettings;
            ClientSize = new Size(imageSettings.Width, imageSettings.Height);

            pictureBox.RecreateImage(imageSettings);
            pictureBox.Dock = DockStyle.Fill;
            Controls.Add(pictureBox);

            var mainMenu = new MenuStrip();
            mainMenu.Items.AddRange(actions.ToMenuItems());
            mainMenu.Dock = DockStyle.Top;
            Controls.Add(mainMenu);
        }

        private static SettingsManager CreateSettingsManager()
        {
            return new SettingsManager(new XmlObjectSerializer(), new FileBlobStorage());
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Text = "Fractal Painter";
        }
    }
}
