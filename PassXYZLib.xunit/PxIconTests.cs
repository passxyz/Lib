using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PassXYZLib.Resources;
using Xunit.Sdk;
using KPCLib;
using KeePassLib;
using System.Collections;
using System.Reflection;

namespace PassXYZLib.xunit
{
    public class ItemGenerator : IEnumerable<object[]>
    {
        private readonly List<Item> items =
                [
                    new PwEntry {Name = "Yahoo"},
                    new PwGroup {Name = "Google"}
                ];

        public IEnumerable<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                items
            };
        }

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            yield return new object[]
            {
                items
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();
    }

    public class PxIconTests
    {
        [Fact]
        public void PxIconDefaultTest()
        { 
            var pxIcon = new PxIcon();
            Debug.WriteLine($"{pxIcon.FontIcon.Glyph}");
            Assert.Equal(pxIcon.FontIcon.Glyph, FontAwesomeRegular.File);
            Assert.NotNull(pxIcon);
        }

        [Fact]
        public void SetFontIconNullReferenceExceptionTest()
        {
            // Arrange
            var fontIcon = new PxFontIcon() 
            { 
                FontFamily = "FontAwesomeBrands", 
                Glyph = FontAwesomeBrands.Yahoo
            };
            var item = new NewItem();

            // Act
            var ex = Assert.Throws<NullReferenceException>(() => item.SetFontIcon(fontIcon));

            // Assert
            Debug.WriteLine(ex.Message );
        }

        [Fact]
        public void SetFontIconInEntryTest() 
        {
            // Arrange
            var fontIcon = new PxFontIcon()
            {
                FontFamily = "FontAwesomeBrands",
                Glyph = FontAwesomeBrands.Yahoo
            };
            var item = new PwEntry();
            // Act
            item.SetFontIcon(fontIcon);
        }

        [Fact]
        public void SetFontIconInGroupTest() 
        {
            var fontIcon = new PxFontIcon()
            {
                FontFamily = "FontAwesomeBrands",
                Glyph = FontAwesomeBrands.Google
            };
            var item = new PwGroup();
            // Act
            item.SetFontIcon(fontIcon);
        }

        [Fact]
        public void GetFontIconNullReferenceExceptionTest()
        {
            // Arrange
            var fontIcon = new PxFontIcon()
            {
                FontFamily = "FontAwesomeBrands",
                Glyph = FontAwesomeBrands.Yahoo
            };
            var item = new NewItem();

            // Act
            var ex = Assert.Throws<NullReferenceException>(() => item.SetFontIcon(fontIcon));
            var icon = item.GetFontIcon();

            // Assert
            Debug.WriteLine(ex.Message);
            Assert.Null(icon);
        }


        [Theory]
        [ClassData(typeof(ItemGenerator))]
        public void GetFontIconTest(List<Item> items) 
        {
            if (items == null)
            {
                ArgumentNullException.ThrowIfNull(items);
            }

            var fontIcon = new PxFontIcon()
            {
                FontFamily = "FontAwesomeBrands",
                Glyph = FontAwesomeBrands.Yahoo
            };

            foreach (Item item in items) 
            {
                // Arrange
                item.SetFontIcon(fontIcon);
                // Act
                var icon = item.GetFontIcon();
                // Assert
                Assert.NotNull(icon);
            }
        }

        [Fact]
        public void GetFontAwesomeBrandsGlyphsTest()
        {
            // Arrange
            var glyphs = FontData.GetGlyphs(typeof(FontAwesomeBrands));
            // Act
            Debug.WriteLine($"{glyphs.Count}");
            // Assert
            Assert.Equal(457, glyphs.Count);
        }

        [Fact]
        public void GetFontAwesomeRegularGlyphsTest() 
        {
            // Arrange
            var glyphs = FontData.GetGlyphs(typeof(FontAwesomeRegular));
            var fontOrderByName = glyphs.OrderBy(x => x.Key);
            // Act
            Debug.WriteLine($"glyphs={glyphs.Count}, fontOrderByName={fontOrderByName.Count()}");
            // Assert
            Assert.Equal(154, glyphs.Count);
        }

        [Fact]
        public void GetFontAwesomeSolidGlyphsTest()
        {
            // Arrange
            var glyphs = FontData.GetGlyphs(typeof(FontAwesomeSolid));
            var arrows = glyphs.Where(x => x.Key.Contains("Arrow")).OrderBy(x => x.Key);
            foreach (var arrow in arrows)
            {
                Debug.WriteLine($"{arrow.Key}");
            }

            // Act
            Debug.WriteLine($"{glyphs.Count}");
            // Assert
            Assert.Equal(915, glyphs.Count);
        }
        [Fact]
        public void FontTypesTest()
        {
            // Arrange
            
            // Act
            Debug.WriteLine($"{nameof(FontType.FontAwesomeBrands)}");
            foreach (var font in FontData.FontFamily)
            {
                Debug.WriteLine($"{font.Value.Name}");
            }
            // Assert
            Assert.Equal(3, FontData.FontFamily.Count);
        }
    }
}
