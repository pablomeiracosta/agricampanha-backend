Add-Type -AssemblyName System.Drawing

$bmp = New-Object System.Drawing.Bitmap(800, 600)
$graphics = [System.Drawing.Graphics]::FromImage($bmp)

$graphics.Clear([System.Drawing.Color]::LightBlue)

$font = New-Object System.Drawing.Font('Arial', 48, [System.Drawing.FontStyle]::Bold)
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::DarkBlue)
$graphics.DrawString('Foto 2', $font, $brush, 280, 250)

$bmp.Save('teste-foto2.jpg', [System.Drawing.Imaging.ImageFormat]::Jpeg)

$graphics.Dispose()
$bmp.Dispose()

Write-Host 'Imagem 2 criada com sucesso'
