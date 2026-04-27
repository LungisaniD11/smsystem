$ErrorActionPreference = 'Stop'

# Create PowerPoint application instance
$ppt = New-Object -ComObject PowerPoint.Application
$ppt.Visible = [Microsoft.Office.Core.MsoTriState]::msoTrue

# Create presentation
$presentation = $ppt.Presentations.Add()

# Helper function to convert RGB
function Get-RGBColor {
    param([int]$R, [int]$G, [int]$B)
    return ($B -shl 16) + ($G -shl 8) + $R
}

# Add slides
Write-Host "Creating Slide 1: Title Slide..."
$slide1 = $presentation.Slides.Add(1, 1)
$slide1.Shapes.Title.TextFrame.Text = "FutureTech Academy"
$slide1.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 54
$slide1.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide1.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 255 255 255

$shape = $slide1.Shapes.AddTextBox(50, 120, 900, 300)
$tf = $shape.TextFrame
$tf.Text = "Student Management System - User Manual v1.0"
$tf.Paragraphs(1).Font.Size = 28
$tf.Paragraphs(1).Font.Color.RGB = Get-RGBColor 255 255 255

# Slide 2: Overview
Write-Host "Creating Slide 2: System Overview..."
$slide2 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide2.Shapes.Title.TextFrame.Text = "System Overview"
$slide2.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide2.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide2.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide2.Shapes.AddTextBox(50, 100, 900, 350)
$tf = $shape.TextFrame
$tf.Text = "Secure cloud-based platform for managing student records, profiles, and enrollment data.`n`nKey Features:`n- Comprehensive Dashboard`n- Google OAuth 2.0 Authentication`n- Complete CRUD Operations`n- Profile Image Management`n- Search and Pagination`n- Enterprise Azure Infrastructure"
$tf.Paragraphs(1).Font.Size = 18

# Slide 3: Getting Started
Write-Host "Creating Slide 3: Getting Started..."
$slide3 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide3.Shapes.Title.TextFrame.Text = "Getting Started: Quick Access"
$slide3.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide3.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide3.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

# Add QR code image
$qrPath = "d:\FutureTech_Academy\StudentManagementSystem\futuretech_qr.png"
if (Test-Path $qrPath) {
    $slide3.Shapes.AddPicture($qrPath, [Microsoft.Office.Core.MsoTriState]::msoFalse, [Microsoft.Office.Core.MsoTriState]::msoCTrue, 350, 120, 250, 250)
    Write-Host "QR code image added to slide"
}

$shape = $slide3.Shapes.AddTextBox(50, 380, 900, 120)
$tf = $shape.TextFrame
$tf.Text = "Scan QR code or visit:`nhttps://futuretechsmsld-fnfgf9evhpghgzbv.southafricanorth-01.azurewebsites.net/"
$tf.Paragraphs(1).Font.Size = 14

# Slide 4: Login
Write-Host "Creating Slide 4: Authentication..."
$slide4 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide4.Shapes.Title.TextFrame.Text = "Step 1: User Authentication"
$slide4.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide4.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide4.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide4.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Login Steps:`n`n1. Open application via QR code or URL`n2. Click Login with Google`n3. Select your Google account`n4. Grant permissions`n5. Redirected to dashboard if authorized`n`nNote: Only registered admin emails can access."
$tf.Paragraphs(1).Font.Size = 16

# Slide 5: Dashboard
Write-Host "Creating Slide 5: Dashboard..."
$slide5 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide5.Shapes.Title.TextFrame.Text = "Step 2: Dashboard Overview"
$slide5.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide5.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide5.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide5.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Dashboard Features:`n`n- Active Students Count`n- Inactive Students Count`n- Students with Profile Images`n- New Students This Week`n`nFunctionality:`n- Quick access to recent students`n- Search by name or student ID`n- Navigation menu for all functions"
$tf.Paragraphs(1).Font.Size = 16

# Slide 6: Adding Students
Write-Host "Creating Slide 6: Adding Students..."
$slide6 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide6.Shapes.Title.TextFrame.Text = "Step 3: Adding a New Student"
$slide6.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide6.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide6.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide6.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Create Student Record:`n`n1. Click Add New Student from dashboard`n2. Enter: First Name, Last Name, Email, Mobile`n3. Select Enrollment Status`n4. Upload profile image (JPEG/PNG, max 5MB, optional)`n5. Click Save`n`nFormats: JPEG and PNG only`n- File signature verification prevents spoofing"
$tf.Paragraphs(1).Font.Size = 16

# Slide 7: Searching
Write-Host "Creating Slide 7: Searching..."
$slide7 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide7.Shapes.Title.TextFrame.Text = "Step 4: Searching & Viewing Students"
$slide7.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide7.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide7.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide7.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Search and View Records:`n`n1. Use search bar by Name or Student ID`n2. Results paginated (10 per page)`n3. Click student name for full details`n4. Profile images shown with secure links`n`nSecurity: Image links expire after 15 minutes`n- Prevents unauthorized access`n- Secure time-limited access to student profiles"
$tf.Paragraphs(1).Font.Size = 16

# Slide 8: Editing
Write-Host "Creating Slide 8: Editing..."
$slide8 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide8.Shapes.Title.TextFrame.Text = "Step 5: Editing Student Records"
$slide8.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide8.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide8.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide8.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Update Student Information:`n`n1. Find student via search or browse`n2. Click Edit button`n3. Update any field (names, email, phone, status)`n4. Replace profile image if needed`n5. Click Save Changes`n`nTip: Leave image field empty to keep existing image"
$tf.Paragraphs(1).Font.Size = 16

# Slide 9: Managing Status
Write-Host "Creating Slide 9: Managing Status..."
$slide9 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide9.Shapes.Title.TextFrame.Text = "Step 6: Managing Student Status"
$slide9.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide9.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide9.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide9.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Two Options Available:`n`nSOFT DELETE (Recommended):`n- Marks student as inactive`n- Record remains in system`n- Data is preserved`n- Can be reactivated`n`nPERMANENT DELETE (Use with caution):`n- Completely removes record`n- Cannot be recovered`n- Requires confirmation"
$tf.Paragraphs(1).Font.Size = 16

# Slide 10: Image Management
Write-Host "Creating Slide 10: Image Management..."
$slide10 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide10.Shapes.Title.TextFrame.Text = "Step 7: Profile Image Management"
$slide10.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide10.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide10.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide10.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Image Upload Guidelines:`n`nSupported: JPEG (.jpg, .jpeg) and PNG (.png)`nSize Limit: Maximum 5 MB per image`nValidation: File signature verification`nProcessing: Images resized for efficiency`n`nUpload Process:`n- Click Choose Image or drag-and-drop`n- System validates format and integrity`n- Image uploaded to secure cloud storage"
$tf.Paragraphs(1).Font.Size = 16

# Slide 11: Security
Write-Host "Creating Slide 11: Security Features..."
$slide11 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide11.Shapes.Title.TextFrame.Text = "Security Features"
$slide11.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide11.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide11.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide11.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Your Data is Protected By:`n`n- Google OAuth 2.0 (Industry standard)`n- Admin Whitelist (Approved users only)`n- HTTPS Encryption (Transit protection)`n- Anti-CSRF Tokens (Attack protection)`n- Security Headers (HSTS, CSP, X-Frame-Options)`n- SAS Links (15-minute image expiry)`n- Azure Cosmos DB (Enterprise protection)`n- Azure Blob Storage (Secure media)"
$tf.Paragraphs(1).Font.Size = 14

# Slide 12: Best Practices
Write-Host "Creating Slide 12: Best Practices..."
$slide12 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide12.Shapes.Title.TextFrame.Text = "Best Practices & Tips"
$slide12.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide12.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide12.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide12.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Usage Guidelines:`n`n- Security: Never share login credentials`n- Data Quality: Enter accurate information`n- Backups: System auto-backs up`n- Soft Delete First: Use soft delete by default`n- Verify Updates: Confirm changes saved`n- Support: Contact IT for technical issues`n- Audit Trail: Use your own account only"
$tf.Paragraphs(1).Font.Size = 16

# Slide 13: Troubleshooting
Write-Host "Creating Slide 13: Troubleshooting..."
$slide13 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide13.Shapes.Title.TextFrame.Text = "Troubleshooting Guide"
$slide13.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide13.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide13.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

$shape = $slide13.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Common Issues:`n`nCannot Login:`n- Verify account registered with admin`n- Check email domain is whitelisted`n- Clear browser cookies`n`nImage Upload Failed:`n- Check JPEG or PNG format`n- Verify file size (max 5MB)`n- Ensure file not corrupted`n`nRecord Not Found:`n- Try different search terms`n- Check if marked as inactive"
$tf.Paragraphs(1).Font.Size = 14

# Slide 14: Summary
Write-Host "Creating Slide 14: Summary..."
$slide14 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide14.Shapes.Title.TextFrame.Text = "You're Ready to Use FutureTech SMS!"
$slide14.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide14.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide14.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = Get-RGBColor 255 255 255

$shape = $slide14.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Text = "Key Capabilities:`n`n- Secure login with Google OAuth`n- Full student record management (CRUD)`n- Profile images with secure access`n- Search and pagination support`n- Enterprise-grade cloud infrastructure`n`nNeed Help?`nContact your system administrator`n`nFutureTech Academy Student Management System | v1.0"
$tf.Paragraphs(1).Font.Size = 16
$tf.Paragraphs(1).Font.Color.RGB = Get-RGBColor 255 255 255

# Save presentation
$outputPath = "d:\FutureTech_Academy\StudentManagementSystem\docs\FutureTech-UserManual.pptx"
Write-Host "Saving presentation to: $outputPath"
$presentation.SaveAs($outputPath, 1)

Write-Host "[SUCCESS] Presentation created successfully!"
Write-Host "Output: $outputPath"

# Close PowerPoint
$presentation.Close()
$ppt.Quit()
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($ppt) | Out-Null

# Verify file
$file = Get-Item $outputPath
Write-Host "`nFile Details:"
Write-Host "Size: $([math]::Round($file.Length/1MB,2)) MB"
Write-Host "Created: $($file.LastWriteTime)"
