Add-Type -AssemblyName System.Drawing

$bmp = New-Object System.Drawing.Bitmap(800, 600)
$graphics = [System.Drawing.Graphics]::FromImage($bmp)

$graphics.Clear([System.Drawing.Color]::LightGreen)

$font = New-Object System.Drawing.Font('Arial', 48, [System.Drawing.FontStyle]::Bold)
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::DarkGreen)
$graphics.DrawString('Teste Auria', $font, $brush, 200, 250)

$bmp.Save('teste-foto.jpg', [System.Drawing.Imaging.ImageFormat]::Jpeg)

$graphics.Dispose()
$bmp.Dispose()

Write-Host 'Imagem criada com sucesso'
