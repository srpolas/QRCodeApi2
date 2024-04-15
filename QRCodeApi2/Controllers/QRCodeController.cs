using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using ZXing;
using ZXing.Common;

namespace QRCodeApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetQRCodeImage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text parameter is required.");
            }

            try
            {
                // Generate QR code
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Height = 300,
                        Width = 300
                    }
                };

                var pixelData = writer.Write(text);
                var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                try
                {
                    // Copy over the color data from the pixel data.
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    // Unlock the bits.
                    bitmap.UnlockBits(bitmapData);
                }

                // Return QR code image
                using (var stream = new System.IO.MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return File(stream.ToArray(), "image/png");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to generate QR code: {ex.Message}");
            }
        }
    }
}
