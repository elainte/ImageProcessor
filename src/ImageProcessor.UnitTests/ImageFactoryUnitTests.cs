﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageFactoryUnitTests.cs" company="James South">
//   Copyright (c) James South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ImageProcessor.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using ImageProcessor.Imaging;
    using ImageProcessor.Imaging.Filters.EdgeDetection;
    using ImageProcessor.Imaging.Filters.Photo;

    using FluentAssertions;
    using NUnit.Framework;

    /// <summary>
    /// Test harness for the image factory
    /// </summary>
    [TestFixture]
    public class ImageFactoryUnitTests
    {
        /// <summary>
        /// The list of images. Designed to speed up the tests a little.
        /// </summary>
        private IEnumerable<FileInfo> imagesInfos;

        /// <summary>
        /// The list of ImageFactories. Designed to speed up the test a bit more.
        /// </summary>
        private List<ImageFactory> imagesFactories;

        /// <summary>
        /// Tests the loading of image from a file
        /// </summary>
        [Test]
        public void TestLoadImageFromFile()
        {
            foreach (FileInfo file in this.ListInputFiles())
            {
                using (ImageFactory imageFactory = new ImageFactory())
                {
                    imageFactory.Load(file.FullName);

                    imageFactory.ImagePath.Should().Be(file.FullName, "because the path should have been memorized");
                    imageFactory.Image.Should().NotBeNull("because the image should have been loaded");
                }
            }
        }

        /// <summary>
        /// Tests the loading of image from a memory stream
        /// </summary>
        [Test]
        public void TestLoadImageFromMemory()
        {
            foreach (FileInfo file in this.ListInputFiles())
            {
                byte[] photoBytes = File.ReadAllBytes(file.FullName);

                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    using (ImageFactory imageFactory = new ImageFactory())
                    {
                        imageFactory.Load(inStream);

                        imageFactory.ImagePath.Should().BeNull("because an image loaded from stream should not have a file path");
                        imageFactory.Image.Should().NotBeNull("because the image should have been loaded");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that the save method actually saves a file
        /// </summary>
        [Test]
        public void TestSaveToDisk()
        {
            foreach (FileInfo file in this.ListInputFiles())
            {
                string outputFileName = string.Format("./output/{0}", file.Name);
                using (ImageFactory imageFactory = new ImageFactory())
                {
                    imageFactory.Load(file.FullName);
                    imageFactory.Save(outputFileName);

                    File.Exists(outputFileName).Should().BeTrue("because the file should have been saved on disk");

                    File.Delete(outputFileName);
                }
            }
        }

        /// <summary>
        /// Tests that the save method actually writes to memory
        /// </summary>
        [Test]
        public void TestSaveToMemory()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                using (MemoryStream s = new MemoryStream())
                {
                    imageFactory.Save(s);
                    s.Seek(0, SeekOrigin.Begin);

                    s.Capacity.Should().BeGreaterThan(0, "because the stream should contain the image");
                }
            }
        }

        /// <summary>
        /// Tests that a filter is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectAlpha()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Alpha(50);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the alpha operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that brightness changes is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectBrightness()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Brightness(50);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the brightness operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that background color changes are really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectBackgroundColor()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.BackgroundColor(Color.Yellow);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the background color operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a contrast change is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectContrast()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Contrast(50);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the contrast operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a saturation change is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectSaturation()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Saturation(50);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the saturation operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a tint change is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectTint()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Tint(Color.FromKnownColor(KnownColor.AliceBlue));
                AssertImagesAreDifferent(original, imageFactory.Image, "because the tint operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a vignette change is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectVignette()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Vignette(Color.FromKnownColor(KnownColor.AliceBlue));
                AssertImagesAreDifferent(original, imageFactory.Image, "because the vignette operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a filter is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectWatermark()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Watermark(new TextLayer
                {
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 10,
                    Position = new Point(10, 10),
                    Text = "Lorem ipsum dolor"
                });
                AssertImagesAreDifferent(original, imageFactory.Image, "because the watermark operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a filter is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectBlur()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.GaussianBlur(5);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the blur operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a filter is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectBlurWithLayer()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.GaussianBlur(new GaussianLayer { Sigma = 10, Size = 5, Threshold = 2 });
                AssertImagesAreDifferent(original, imageFactory.Image, "because the layered blur operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a filter is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectSharpen()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.GaussianSharpen(5);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the sharpen operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that a filter is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestApplyEffectSharpenWithLayer()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.GaussianSharpen(new GaussianLayer { Sigma = 10, Size = 5, Threshold = 2 });
                AssertImagesAreDifferent(original, imageFactory.Image, "because the layered sharpen operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that all filters can be applied
        /// </summary>
        [Test]
        public void TestApplyEffectFilter()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();

                List<IMatrixFilter> filters = new List<IMatrixFilter>()
                {
                    MatrixFilters.BlackWhite,
                    MatrixFilters.Comic,
                    MatrixFilters.Gotham,
                    MatrixFilters.GreyScale,
                    MatrixFilters.HiSatch,
                    MatrixFilters.Invert,
                    MatrixFilters.Lomograph,
                    MatrixFilters.LoSatch,
                    MatrixFilters.Polaroid,
                    MatrixFilters.Sepia
                };

                foreach (IMatrixFilter filter in filters)
                {
                    imageFactory.Filter(filter);
                    AssertImagesAreDifferent(original, imageFactory.Image, "because the filter operation should have been applied on {0}", imageFactory.ImagePath);
                    imageFactory.Reset();
                    AssertImagesAreIdentical(original, imageFactory.Image, "because the image should be reset");
                }
            }
        }

        /// <summary>
        /// Tests that a filter is really applied by checking that the image is modified
        /// </summary>
        [Test]
        public void TestRoundedCorners()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.RoundedCorners(new RoundedCornerLayer(5));
                AssertImagesAreDifferent(original, imageFactory.Image, "because the rounded corners operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that the image is well resized using constraints
        /// </summary>
        [Test]
        public void TestResizeConstraints()
        {
            const int MaxSize = 200;
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                imageFactory.Constrain(new Size(MaxSize, MaxSize));
                imageFactory.Image.Width.Should().BeLessOrEqualTo(MaxSize, "because the image size should have been reduced");
                imageFactory.Image.Height.Should().BeLessOrEqualTo(MaxSize, "because the image size should have been reduced");
            }
        }

        /// <summary>
        /// Tests that the image is well cropped
        /// </summary>
        [Test]
        public void TestCrop()
        {
            const int MaxSize = 20;
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Crop(new Rectangle(0, 0, MaxSize, MaxSize));
                AssertImagesAreDifferent(original, imageFactory.Image, "because the crop operation should have been applied on {0}", imageFactory.ImagePath);

                imageFactory.Image.Width.Should().Be(MaxSize, "because the cropped image should be {0}x{0}", MaxSize);
                imageFactory.Image.Height.Should().Be(MaxSize, "because the cropped image should be {0}x{0}", MaxSize);
            }
        }

        /// <summary>
        /// Tests that the image is well cropped
        /// </summary>
        [Test]
        public void TestCropWithCropLayer()
        {
            const int MaxSize = 20;
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Crop(new CropLayer(0, 0, MaxSize, MaxSize, CropMode.Pixels));
                AssertImagesAreDifferent(original, imageFactory.Image, "because the layered crop operation should have been applied on {0}", imageFactory.ImagePath);

                imageFactory.Image.Width.Should().Be(MaxSize, "because the cropped image should be {0}x{0}", MaxSize);
                imageFactory.Image.Height.Should().Be(MaxSize, "because the cropped image should be {0}x{0}", MaxSize);
            }
        }

        /// <summary>
        /// Tests that the image is flipped
        /// </summary>
        [Test]
        public void TestFlip()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Flip(true);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the vertical flip operation should have been applied on {0}", imageFactory.ImagePath);
                imageFactory.Image.Width.Should().Be(original.Width, "because the dimensions should not have changed");
                imageFactory.Image.Height.Should().Be(original.Height, "because the dimensions should not have changed");
                imageFactory.Reset();
                AssertImagesAreIdentical(original, imageFactory.Image, "because the image should be reset");

                imageFactory.Flip();
                AssertImagesAreDifferent(original, imageFactory.Image, "because the horizontal flip operation should have been applied on {0}", imageFactory.ImagePath);
                imageFactory.Image.Width.Should().Be(original.Width, "because the dimensions should not have changed");
                imageFactory.Image.Height.Should().Be(original.Height, "because the dimensions should not have changed");
            }
        }

        /// <summary>
        /// Tests that the image is resized
        /// </summary>
        [Test]
        public void TestResize()
        {
            const int NewSize = 150;
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                imageFactory.Resize(new Size(NewSize, NewSize));

                imageFactory.Image.Width.Should().Be(NewSize, "because the new image's size should be {0}x{0}", NewSize);
                imageFactory.Image.Height.Should().Be(NewSize, "because the new image's size should be {0}x{0}", NewSize);
            }
        }

        /// <summary>
        /// Tests that the image is resized
        /// </summary>
        [Test]
        public void TestResizeWithLayer()
        {
            const int NewSize = 150;
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                imageFactory.Resize(new ResizeLayer(new Size(NewSize, NewSize), ResizeMode.Stretch, AnchorPosition.Left));

                imageFactory.Image.Width.Should().Be(NewSize, "because the new image's size should be {0}x{0}", NewSize);
                imageFactory.Image.Height.Should().Be(NewSize, "because the new image's size should be {0}x{0}", NewSize);
            }
        }

        /// <summary>
        /// Tests that the image is resized
        /// </summary>
        [Test]
        public void TestRotate()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Rotate(90);

                imageFactory.Image.Width.Should().Be(original.Height, "because the rotated image dimensions should have been switched");
                imageFactory.Image.Height.Should().Be(original.Width, "because the rotated image dimensions should have been switched");
            }
        }

        /// <summary>
        /// Tests that the images hue has been altered.
        /// </summary>
        [Test]
        public void TestHue()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Hue(90);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the hue operation should have been applied on {0}", imageFactory.ImagePath);

                imageFactory.Reset();
                AssertImagesAreIdentical(original, imageFactory.Image, "because the image should be reset");

                imageFactory.Hue(116, true);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the hue+rotate operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that the image has been pixelated.
        /// </summary>
        [Test]
        public void TestPixelate()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.Pixelate(8);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the pixelate operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that the images quality has been set.
        /// </summary>
        [Test]
        public void TestQuality()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                int original = imageFactory.CurrentImageFormat.Quality;
                imageFactory.Quality(69);
                int updated = imageFactory.CurrentImageFormat.Quality;

                updated.Should().NotBe(original, "because the quality should have been changed");
            }
        }

        /// <summary>
        /// Tests that the image has had a color replaced.
        /// </summary>
        [Test]
        public void TestReplaceColor()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();
                imageFactory.ReplaceColor(Color.White, Color.Black, 90);
                AssertImagesAreDifferent(original, imageFactory.Image, "because the color replace operation should have been applied on {0}", imageFactory.ImagePath);
            }
        }

        /// <summary>
        /// Tests that the various edge detection algorithms are applied.
        /// </summary>
        [Test]
        public void TestEdgeDetection()
        {
            foreach (ImageFactory imageFactory in this.ListInputImages())
            {
                Image original = (Image)imageFactory.Image.Clone();

                List<IEdgeFilter> filters = new List<IEdgeFilter>()
                {
                    new KayyaliEdgeFilter(),
                    new KirschEdgeFilter(),
                    new Laplacian3X3EdgeFilter(),
                    new Laplacian5X5EdgeFilter(),
                    new LaplacianOfGaussianEdgeFilter(),
                    new PrewittEdgeFilter(),
                    new RobertsCrossEdgeFilter(),
                    new ScharrEdgeFilter(),
                    new SobelEdgeFilter()
                };

                foreach (IEdgeFilter filter in filters)
                {
                    imageFactory.DetectEdges(filter);
                    AssertImagesAreDifferent(original, imageFactory.Image, "because the edge operation should have been applied on {0}", imageFactory.ImagePath);
                    imageFactory.Reset();
                    AssertImagesAreIdentical(original, imageFactory.Image, "because the image should be reset");
                }
            }
        }

        /// <summary>
        /// Gets the files matching the given extensions.
        /// </summary>
        /// <param name="dir">
        /// The <see cref="System.IO.DirectoryInfo"/>.
        /// </param>
        /// <param name="extensions">
        /// The extensions.
        /// </param>
        /// <returns>
        /// A collection of <see cref="System.IO.FileInfo"/>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The extensions variable is null.
        /// </exception>
        private static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions");
            }

            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lists the input files in the Images folder
        /// </summary>
        /// <returns>The list of files.</returns>
        private IEnumerable<FileInfo> ListInputFiles()
        {
            if (this.imagesInfos != null)
            {
                return this.imagesInfos;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo("./Images");

            this.imagesInfos = GetFilesByExtensions(directoryInfo, new[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp", ".webp" });

            return this.imagesInfos;
        }

        /// <summary>
        /// Lists the input images to use from the Images folder
        /// </summary>
        /// <returns>The list of images</returns>
        private IEnumerable<ImageFactory> ListInputImages()
        {
            if (imagesFactories == null || imagesFactories.Count() == 0)
            {
                imagesFactories = new List<ImageFactory>();
                foreach (FileInfo fi in this.ListInputFiles())
                {
                    imagesFactories.Add((new ImageFactory()).Load(fi.FullName));
                }
            }

            // reset all the images whenever we call this
            foreach (ImageFactory image in imagesFactories)
            {
                image.Reset();
            }

            return imagesFactories;
        }

        /// <summary>
        /// Asserts that two images are identical
        /// </summary>
        /// <param name="expected">The expected result</param>
        /// <param name="tested">The tested image</param>
        private void AssertImagesAreIdentical(Image expected, Image tested, string because, params string[] becauseArgs)
        {
            ToByteArray(expected).SequenceEqual(ToByteArray(tested)).Should().BeTrue(because, becauseArgs);
        }

        /// <summary>
        /// Asserts that two images are different
        /// </summary>
        /// <param name="expected">The not-expected result</param>
        /// <param name="tested">The tested image</param>
        private void AssertImagesAreDifferent(Image expected, Image tested, string because, params string[] becauseArgs)
        {
            ToByteArray(expected).SequenceEqual(ToByteArray(tested)).Should().BeFalse(because, becauseArgs);
        }

        /// <summary>
        /// Converts an image to a byte array
        /// </summary>
        /// <param name="imageIn">The image to convert</param>
        /// <param name="format">The format to use</param>
        /// <returns>An array of bytes representing the image</returns>
        public static byte[] ToByteArray(Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}