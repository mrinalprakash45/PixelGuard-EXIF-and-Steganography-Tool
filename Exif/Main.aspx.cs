using System;
using System.Drawing;
using System.Web;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Exif
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private string ConvertToDegrees(byte[] value)
        {
            // Conversion logic for GPS coordinates
            uint degreesNumerator = BitConverter.ToUInt32(value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(value, 4);
            double degrees = degreesNumerator / (double)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(value, 12);
            double minutes = minutesNumerator / (double)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(value, 20);
            double seconds = secondsNumerator / (double)secondsDenominator;

            // Combine degrees, minutes, and seconds to a single degree value
            double coordinate = degrees + (minutes / 60.0) + (seconds / 3600.0);

            return coordinate.ToString("G6"); // Format the coordinate to 6 significant digits
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                try
                {
                    HttpPostedFile postedFile = fileUpload.PostedFile;

                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(postedFile.InputStream))
                    {
                        string latitudeRef = string.Empty;
                        string longitudeRef = string.Empty;
                        string latitude = string.Empty;
                        string longitude = string.Empty;

                        foreach (PropertyItem propItem in image.PropertyItems)
                        {
                            if (propItem.Id == 0x0001) // LatitudeRef
                            {
                                latitudeRef = System.Text.Encoding.ASCII.GetString(propItem.Value).Trim('\0');
                            }
                            else if (propItem.Id == 0x0002) // Latitude
                            {
                                latitude = ConvertToDegrees(propItem.Value);
                            }
                            else if (propItem.Id == 0x0003) // LongitudeRef
                            {
                                longitudeRef = System.Text.Encoding.ASCII.GetString(propItem.Value).Trim('\0');
                            }
                            else if (propItem.Id == 0x0004) // Longitude
                            {
                                longitude = ConvertToDegrees(propItem.Value);
                            }
                        }

                        string locationInfo = string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude)
                            ? "Location information not available.<br/>"
                            : $"Location: Latitude {latitude} {latitudeRef}, Longitude {longitude} {longitudeRef}<br/>";

                        lblMetadata.Text = $@"
                            File Size: {postedFile.ContentLength} bytes<br/>
                            MIME Type: {postedFile.ContentType}<br/>
                            Image Width: {image.Width}px<br/>
                            Image Height: {image.Height}px<br/>
                            Bit Depth: {System.Drawing.Image.GetPixelFormatSize(image.PixelFormat)}<br/>
                            Image Size: {image.Width}x{image.Height}<br/>
                            Megapixels: {(image.Width * image.Height) / 1000000.0} MP
                        ";
                    }
                }
                catch (Exception ex)
                {
                    lblMetadata.Text = $"Error: {ex.Message}";
                }
            }
        }
        private string GetPropertyValue(PropertyItem property)
        {
            // Convert property value to string based on property type
            // This is a simplified example, you may need to handle different property types accordingly
            return Encoding.UTF8.GetString(property.Value).Trim('\0');
        }

        private string ExtractMessage(Bitmap bitmap)
        {
            // Simplified example to extract hidden message from the image
            // You may need a more sophisticated method to handle different steganography techniques
            StringBuilder message = new StringBuilder();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        break;
                    }
                    char character = (char)pixel.R;
                    message.Append(character);
                }
            }
            return message.ToString();
        }
        protected void btnEncode_Click(object sender, EventArgs e)
        {
            if (fileUpload1.HasFile && chkSteganography.Checked && !string.IsNullOrEmpty(txtMessage.Text))
            {
                try
                {
                    // Read the uploaded file into a Bitmap object
                    using (Bitmap bitmap = new Bitmap(fileUpload1.PostedFile.InputStream))
                    {
                        // Convert the message to a char array
                        char[] message = txtMessage.Text.ToCharArray();

                        // Initialize variables to keep track of the position in the image and the message
                        int messageIndex = 0;
                        int charValueIndex = 0;

                        // Loop over the pixels of the image to encode the message
                        for (int i = 0; i < bitmap.Height; i++)
                        {
                            for (int j = 0; j < bitmap.Width; j++)
                            {
                                // Break out of the loops if the entire message has been encoded
                                if (messageIndex >= message.Length)
                                    break;

                                // Get the pixel at the current position
                                Color pixel = bitmap.GetPixel(j, i);

                                // Get the ASCII value of the current character in the message
                                int charValue = message[messageIndex];

                                // Encode the next bit of the character in the red, green, and blue values of the pixel
                                int newR = (pixel.R & 0xFE) | ((charValue >> (charValueIndex * 3)) & 1);
                                int newG = (pixel.G & 0xFE) | ((charValue >> (charValueIndex * 3 + 1)) & 1);
                                int newB = (pixel.B & 0xFE) | ((charValue >> (charValueIndex * 3 + 2)) & 1);

                                // Set the pixel with the modified values
                                bitmap.SetPixel(j, i, Color.FromArgb(newR, newG, newB));

                                // Update the position in the message and the character
                                if (++charValueIndex >= 3)
                                {
                                    charValueIndex = 0;
                                    messageIndex++;
                                }
                            }

                            if (messageIndex >= message.Length)
                                break;
                        }

                        // Convert the modified bitmap to a byte array
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] imageBytes = ms.ToArray();

                            // Send the byte array as a response with the appropriate headers for download
                            Response.Clear();
                            Response.ContentType = "image/png";
                            Response.AddHeader("Content-Disposition", "attachment; filename=EncodedImage.png");
                            Response.BinaryWrite(imageBytes);
                            Response.End();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately, possibly by displaying an error message to the user
                    Response.Write($"<script>alert('{ex.Message.Replace("'", "\\'")}')<script>");
                }
            }
            else
            {
                // Inform the user if the file is not uploaded, the checkbox is not checked, or the message is empty
                Response.Write("<script>alert('Please upload a file, check the checkbox, and enter a message to encode.')<script>");
            }
        }
        protected void btnDecode_Click(object sender, EventArgs e)
        {
            if (fileUpload1.HasFile)
            {
                try
                {
                    // Read the uploaded file into a Bitmap object
                    using (Bitmap bitmap = new Bitmap(fileUpload1.PostedFile.InputStream))
                    {
                        // Initialize variables to keep track of the position in the image and the message
                        int messageIndex = 0;
                        int charValueIndex = 0;
                        int charValue = 0;
                        List<char> message = new List<char>();

                        // Loop over the pixels of the image to decode the message
                        for (int i = 0; i < bitmap.Height; i++)
                        {
                            for (int j = 0; j < bitmap.Width; j++)
                            {
                                // Get the pixel at the current position
                                Color pixel = bitmap.GetPixel(j, i);

                                // Extract the bits of the character from the red, green, and blue values of the pixel
                                charValue |= ((pixel.R & 1) << (charValueIndex * 3));
                                charValue |= ((pixel.G & 1) << (charValueIndex * 3 + 1));
                                charValue |= ((pixel.B & 1) << (charValueIndex * 3 + 2));

                                // Update the position in the message and the character
                                if (++charValueIndex >= 3)
                                {
                                    charValueIndex = 0;
                                    message.Add((char)charValue);
                                    charValue = 0;

                                    // You may want to have a special character or sequence to denote the end of the message
                                    if (message.Count >= 2 && message[message.Count - 2] == '#' && message[message.Count - 1] == '#')
                                        break;
                                }
                            }

                            // Break out of the loop if the end of the message has been reached
                            if (message.Count >= 2 && message[message.Count - 2] == '#' && message[message.Count - 1] == '#')
                                break;
                        }

                        // Convert the message to a string and display it to the user
                        string decodedMessage = new string(message.ToArray());
                        lblMessage.Text = decodedMessage.TrimEnd('#');
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately, possibly by displaying an error message to the user
                    Response.Write($"<script>alert('{ex.Message.Replace("'", "\\'")}')<script>");
                }
            }
            else
            {
                // Inform the user if the file is not uploaded
                Response.Write("<script>alert('Please upload an encoded image to decode.')<script>");
            }
        }
    }
}