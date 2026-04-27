$ErrorActionPreference = 'Stop'

# Create PowerPoint application instance
$ppt = New-Object -ComObject PowerPoint.Application
$ppt.Visible = $true

# Create presentation
$presentation = $ppt.Presentations.Add()

# Define color scheme
$accentColor = RGB(15, 118, 110)  # Teal
$darkColor = RGB(31, 58, 86)      # Dark blue
$lightBg = RGB(230, 255, 250)     # Light teal

function Add-Title-Slide {
    param([object]$pres, [string]$title, [string]$subtitle)
    
    $slide = $pres.Slides.Add(1, [Microsoft.Office.Interop.PowerPoint.PpSlideLayout]::ppLayoutTitle)
    $slide.Background.Fill.Solid()
    $slide.Background.Fill.ForeColor.RGB = RGB(15, 118, 110)
    
    $titleShape = $slide.Shapes.Title
    $titleShape.TextFrame.Text = $title
    $titleShape.TextFrame.Paragraphs(1).Font.Size = 54
    $titleShape.TextFrame.Paragraphs(1).Font.Bold = $true
    $titleShape.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(255, 255, 255)
    
    $subtitleShape = $slide.Shapes(2)
    $subtitleShape.TextFrame.Text = $subtitle
    $subtitleShape.TextFrame.Paragraphs(1).Font.Size = 24
    $subtitleShape.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(255, 255, 255)
    
    return $slide
}

function Add-Content-Slide {
    param([object]$pres, [string]$title, [string[]]$content)
    
    $slide = $pres.Slides.Add($pres.Slides.Count + 1, [Microsoft.Office.Interop.PowerPoint.PpSlideLayout]::ppLayoutTitleContent)
    
    $titleShape = $slide.Shapes.Title
    $titleShape.TextFrame.Text = $title
    $titleShape.TextFrame.Paragraphs(1).Font.Size = 44
    $titleShape.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)
    $titleShape.TextFrame.Paragraphs(1).Font.Bold = $true
    
    # Add content
    for ($i = 0; $i -lt $content.Count; $i++) {
        if ($i -eq 0) {
            $bodyShape = $slide.Shapes(2)
            $bodyShape.TextFrame.Text = $content[$i]
        } else {
            $tf = $bodyShape.TextFrame
            $para = $tf.AddParagraph()
            $para.Text = $content[$i]
        }
    }
    
    return $slide
}

# Slide 1: Title Slide
Write-Host "Creating Slide 1: Title..."
Add-Title-Slide $presentation "FutureTech Academy" "Student Management System - User Manual v1.0"

# Slide 2: System Overview
Write-Host "Creating Slide 2: Overview..."
$slide2 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide2.Shapes.Title.TextFrame.Text = "System Overview"
$slide2.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide2.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide2.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide2.Shapes.AddTextBox(50, 100, 900, 350)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "A secure, cloud-based platform for managing student records, profiles, and enrollment data.`n`nKey Features:`n• Comprehensive Dashboard with real-time metrics`n• Secure Google OAuth 2.0 authentication`n• Complete CRUD operations for student records`n• Profile image management with secure access`n• Search and pagination capabilities"
$tf.Paragraphs(1).Font.Size = 18

# Slide 3: Getting Started - QR Code
Write-Host "Creating Slide 3: Getting Started..."
$slide3 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide3.Shapes.Title.TextFrame.Text = "Getting Started: Quick Access"
$slide3.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide3.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide3.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

# Add QR code image
$qrPath = "d:\FutureTech_Academy\StudentManagementSystem\futuretech_qr.png"
if (Test-Path $qrPath) {
    $slide3.Shapes.AddPicture($qrPath, [Microsoft.Office.Core.MsoTriState]::msoFalse, [Microsoft.Office.Core.MsoTriState]::msoCTrue, 350, 120, 250, 250)
}

$shape = $slide3.Shapes.AddTextBox(50, 380, 900, 120)
$tf = $shape.TextFrame
$tf.Text = "Scan QR code to access the system`n`nOr visit: https://futuretechsmsld-fnfgf9evhpghgzbv.southafricanorth-01.azurewebsites.net/"
$tf.Paragraphs(1).Font.Size = 16

# Slide 4: Login Process
Write-Host "Creating Slide 4: Login Process..."
$slide4 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide4.Shapes.Title.TextFrame.Text = "Step 1: User Authentication"
$slide4.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide4.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide4.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide4.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Login Steps:`n`n1. Open the application using QR code or direct URL`n2. Click Login with Google button`n3. Select your Google account`n4. Grant necessary permissions`n5. You will be redirected to dashboard if authorized`n`n[WARNING] Note: Only registered admin emails can access the system."
$tf.Paragraphs(1).Font.Size = 16

# Slide 5: Dashboard Overview
Write-Host "Creating Slide 5: Dashboard..."
$slide5 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide5.Shapes.Title.TextFrame.Text = "Step 2: Dashboard Overview"
$slide5.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide5.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide5.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide5.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Dashboard Features:`n`n[Active] Count of active enrolled students`n[Inactive] Number of inactive records`n[Images] Records with profile photos`n[New] Recently added student records`n`nFunctionality:`n* Quick access to recent students (latest 12)`n* Search bar to find students by name or ID`n* Main navigation menu for all functions"
$tf.Paragraphs(1).Font.Size = 16

# Slide 6: Adding Students
Write-Host "Creating Slide 6: Adding Students..."
$slide6 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide6.Shapes.Title.TextFrame.Text = "Step 3: Adding a New Student"
$slide6.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide6.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide6.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide6.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Create Student Record:`n`n1. Click '+Add New Student' from dashboard`n2. Fill required fields: First Name, Last Name, Email, Mobile`n3. Select Enrollment Status (Active/Inactive)`n4. Upload profile image (JPEG/PNG, optional, max 5MB)`n5. Click 'Save' to create record`n`n[OK] Valid Formats: JPEG and PNG only`n[OK] File validation includes signature verification"
$tf.Paragraphs(1).Font.Size = 16

# Slide 7: Searching Students
Write-Host "Creating Slide 7: Searching..."
$slide7 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide7.Shapes.Title.TextFrame.Text = "Step 4: Searching & Viewing Students"
$slide7.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide7.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide7.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide7.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Search and View Records:`n`n1. Use search bar to query by Name or Student ID`n2. Results display with pagination (10 per page)`n3. Click student name to view full details`n4. Profile images appear with secure access links`n`n[LOCK] Security: Image links expire after 15 minutes`n This prevents unauthorized long-term access to student profiles"
$tf.Paragraphs(1).Font.Size = 16

# Slide 8: Editing Students
Write-Host "Creating Slide 8: Editing..."
$slide8 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide8.Shapes.Title.TextFrame.Text = "Step 5: Editing Student Records"
$slide8.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide8.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide8.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide8.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Update Student Information:`n`n1. Find student using search or browse list`n2. Click Edit button next to the record`n3. Update any field (names, email, phone, status)`n4. Replace profile image if needed (optional)`n5. Click Save Changes to update`n`n[TIP] Leave image field empty to keep existing image"
$tf.Paragraphs(1).Font.Size = 16

# Slide 9: Managing Records
Write-Host "Creating Slide 9: Managing Status..."
$slide9 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide9.Shapes.Title.TextFrame.Text = "Step 6: Managing Student Status"
$slide9.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide9.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide9.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide9.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Two Options Available:`n`nSOFT DELETE (Recommended):`n- Marks student as inactive`n- Record remains in system`n- Data is preserved`n- Can be reactivated later`n`nPERMANENT DELETE (Use with caution):`n- Completely removes record`n- Cannot be recovered`n- Requires confirmation`n- Use only when certain"
$tf.Paragraphs(1).Font.Size = 16

# Slide 10: Image Management
Write-Host "Creating Slide 10: Images..."
$slide10 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide10.Shapes.Title.TextFrame.Text = "Step 7: Profile Image Management"
$slide10.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide10.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide10.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide10.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Image Upload Guidelines:`n`n[OK] Supported Formats: JPEG (.jpg, .jpeg) and PNG (.png)`n[OK] Size Limit: Maximum 5 MB per image`n[OK] Validation: File signature verification prevents spoofing`n[OK] Auto-Processing: Images resized for efficiency`n`nUpload Process:`n- Click Choose Image or drag-and-drop`n- System validates format and integrity`n- Image uploaded to secure cloud storage`n- Link stored with student record"
$tf.Paragraphs(1).Font.Size = 16

# Slide 11: Security
Write-Host "Creating Slide 11: Security..."
$slide11 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide11.Shapes.Title.TextFrame.Text = "Security Features"
$slide11.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide11.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide11.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide11.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Your Data is Protected By:`n`n[LOCK] Google OAuth 2.0 - Industry standard authentication`n[LOCK] Admin Whitelist - Only approved users access system`n[LOCK] HTTPS Encryption - All traffic encrypted in transit`n[LOCK] Anti-CSRF Tokens - Protection against attacks`n[LOCK] Security Headers - HSTS, CSP, X-Frame-Options`n[LOCK] SAS Links - Time-limited image access (15-min expiry)`n[LOCK] Azure Cosmos DB - Enterprise-grade data protection`n[LOCK] Azure Blob Storage - Secure media management"
$tf.Paragraphs(1).Font.Size = 14

# Slide 12: Best Practices
Write-Host "Creating Slide 12: Best Practices..."
$slide12 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide12.Shapes.Title.TextFrame.Text = "Best Practices & Tips"
$slide12.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide12.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide12.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide12.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Usage Guidelines:`n`n[YES] Security: Never share login credentials, use your own account`n[YES] Data Quality: Enter accurate information, verify before saving`n[YES] Backups: System auto-backs up, report discrepancies immediately`n[YES] Soft Delete First: Use soft delete by default`n[YES] Verify Updates: Always confirm changes were saved`n[YES] Support: Contact IT for access or technical issues"
$tf.Paragraphs(1).Font.Size = 16

# Slide 13: Troubleshooting
Write-Host "Creating Slide 13: Troubleshooting..."
$slide13 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide13.Shapes.Title.TextFrame.Text = "Troubleshooting Guide"
$slide13.Shapes.Title.TextFrame.Paragraphs(1).Font.Size = 44
$slide13.Shapes.Title.TextFrame.Paragraphs(1).Font.Bold = $true
$slide13.Shapes.Title.TextFrame.Paragraphs(1).Font.Color.RGB = RGB(15, 118, 110)

$shape = $slide13.Shapes.AddTextBox(50, 100, 900, 380)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "Common Issues:`n`nCannot Login?`n• Verify account is registered with admin`n• Check email domain is whitelisted`n• Clear browser cookies and retry`n`nImage Upload Failed?`n• Ensure file is JPEG or PNG`n• Check file size (max 5MB)`n• Verify file is not corrupted`n`nRecord Not Found?`n• Try different search terms`n• Check if marked as inactive`n• Verify student ID format"
$tf.Paragraphs(1).Font.Size = 14

# Slide 14: Summary
Write-Host "Creating Slide 14: Summary..."
$slide14 = $presentation.Slides.Add($presentation.Slides.Count + 1, 1)
$slide14.Background.Fill.Solid()
$slide14.Background.Fill.ForeColor.RGB = RGB(15, 118, 110)

$shape = $slide14.Shapes.AddTextBox(50, 80, 900, 420)
$tf = $shape.TextFrame
$tf.Word.Wrap = -1
$tf.Text = "You're Ready to Use FutureTech SMS!`n`n[CHECK] Secure login with Google OAuth`n[CHECK] Full student record management (CRUD)`n[CHECK] Profile images with secure access`n[CHECK] Search and pagination support`n[CHECK] Enterprise-grade cloud infrastructure`n`nNeed Help?`nContact your system administrator`n`nFutureTech Academy Student Management System | v1.0"
$tf.Paragraphs(1).Font.Size = 18
$tf.Paragraphs(1).Font.Color.RGB = RGB(255, 255, 255)
$tf.Paragraphs(1).Font.Bold = $true

# Save presentation
$outputPath = "d:\FutureTech_Academy\StudentManagementSystem\docs\FutureTech-UserManual.pptx"
Write-Host "Saving presentation to: $outputPath"
$presentation.SaveAs($outputPath, 1)

Write-Host "[OK] Presentation created successfully!"
Write-Host "Location: $outputPath"

# Get file info
Get-Item $outputPath | Select-Object FullName, @{Name='SizeMB';Expression={[math]::Round($_.Length/1MB,2)}}, LastWriteTime
