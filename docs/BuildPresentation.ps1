$ErrorActionPreference = 'Stop'

# Create PowerPoint application
$ppt = New-Object -ComObject PowerPoint.Application
$ppt.Visible = -1

# Add presentation
$presentation = $ppt.Presentations.Add()

# RGB Color Helper
function Get-RGBColor {
    param([int]$R, [int]$G, [int]$B)
    return ($B -shl 16) + ($G -shl 8) + $R
}

function Add-TitleSlide {
    param([string]$Title, [string]$Subtitle)
    $slide = $presentation.Slides.Add($presentation.Slides.Count + 1, 6)
    
    # Add title
    $shape = $slide.Shapes.AddTextBox(50, 150, 860, 150)
    $tf = $shape.TextFrame
    $tf.Text = $Title
    $tf.Paragraphs(1).Font.Size = 54
    $tf.Paragraphs(1).Font.Bold = $true
    $tf.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110
    
    # Add subtitle
    $shape = $slide.Shapes.AddTextBox(50, 320, 860, 150)
    $tf = $shape.TextFrame
    $tf.Text = $Subtitle
    $tf.Paragraphs(1).Font.Size = 28
    $tf.Paragraphs(1).Font.Color.RGB = Get-RGBColor 80 80 80
}

function Add-ContentSlide {
    param([string]$Title, [string]$Content)
    $slide = $presentation.Slides.Add($presentation.Slides.Count + 1, 6)
    
    # Add title
    $shape = $slide.Shapes.AddTextBox(50, 30, 860, 60)
    $tf = $shape.TextFrame
    $tf.Text = $Title
    $tf.Paragraphs(1).Font.Size = 44
    $tf.Paragraphs(1).Font.Bold = $true
    $tf.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110
    
    # Add content
    $shape = $slide.Shapes.AddTextBox(50, 100, 860, 380)
    $tf = $shape.TextFrame
    $tf.Word.Wrap = $true
    $tf.Text = $Content
    $tf.Paragraphs(1).Font.Size = 16
    $tf.Paragraphs(1).Font.Color.RGB = Get-RGBColor 50 50 50
}

# Create slides
Write-Host "Generating 14-slide presentation..."

# Slide 1
Write-Host "  Slide 1: Title..."
Add-TitleSlide "FutureTech Academy" "Student Management System - User Manual v1.0"

# Slide 2
Write-Host "  Slide 2: Overview..."
Add-ContentSlide "System Overview" "Secure cloud-based platform for managing student records, profiles, and enrollment data.`n`nKey Features:`n- Comprehensive Dashboard`n- Google OAuth 2.0 Authentication`n- Complete CRUD Operations`n- Profile Image Management`n- Search and Pagination`n- Enterprise Azure Infrastructure"

# Slide 3
Write-Host "  Slide 3: Getting Started..."
$slide3 = $presentation.Slides.Add($presentation.Slides.Count + 1, 6)
$shape = $slide3.Shapes.AddTextBox(50, 30, 860, 60)
$tf = $shape.TextFrame
$tf.Text = "Getting Started: Quick Access"
$tf.Paragraphs(1).Font.Size = 44
$tf.Paragraphs(1).Font.Bold = $true
$tf.Paragraphs(1).Font.Color.RGB = Get-RGBColor 15 118 110

if (Test-Path "d:\FutureTech_Academy\StudentManagementSystem\futuretech_qr.png") {
    $slide3.Shapes.AddPicture("d:\FutureTech_Academy\StudentManagementSystem\futuretech_qr.png", $false, $true, 350, 120, 250, 250)
}

$shape = $slide3.Shapes.AddTextBox(50, 380, 860, 120)
$tf = $shape.TextFrame
$tf.Text = "Scan QR code to access the system`n`nhttps://futuretechsmsld-fnfgf9evhpghgzbv.southafricanorth-01.azurewebsites.net/"
$tf.Paragraphs(1).Font.Size = 14

# Slide 4
Write-Host "  Slide 4: Authentication..."
Add-ContentSlide "Step 1: User Authentication" "Login Steps:`n`n1. Open application via QR code or URL`n2. Click Login with Google`n3. Select your Google account`n4. Grant permissions`n5. Redirected to dashboard if authorized`n`nNote: Only registered admin emails can access."

# Slide 5
Write-Host "  Slide 5: Dashboard..."
Add-ContentSlide "Step 2: Dashboard Overview" "Dashboard Features:`n`n- Active Students Count`n- Inactive Students Count`n- Students with Profile Images`n- New Students This Week`n`nFunctionality:`n- Quick access to recent students`n- Search by name or student ID`n- Navigation menu for all functions"

# Slide 6
Write-Host "  Slide 6: Adding Students..."
Add-ContentSlide "Step 3: Adding a New Student" "Create Student Record:`n`n1. Click Add New Student from dashboard`n2. Enter: First Name, Last Name, Email, Mobile`n3. Select Enrollment Status`n4. Upload profile image (JPEG/PNG, max 5MB, optional)`n5. Click Save`n`nFormats: JPEG and PNG only`nFile signature verification prevents spoofing"

# Slide 7
Write-Host "  Slide 7: Searching..."
Add-ContentSlide "Step 4: Searching & Viewing Students" "Search and View Records:`n`n1. Use search bar by Name or Student ID`n2. Results paginated (10 per page)`n3. Click student name for full details`n4. Profile images shown with secure links`n`nSecurity: Image links expire after 15 minutes`nPrevents unauthorized access to student profiles"

# Slide 8
Write-Host "  Slide 8: Editing..."
Add-ContentSlide "Step 5: Editing Student Records" "Update Student Information:`n`n1. Find student via search or browse`n2. Click Edit button`n3. Update any field (names, email, phone, status)`n4. Replace profile image if needed`n5. Click Save Changes`n`nTip: Leave image field empty to keep existing image"

# Slide 9
Write-Host "  Slide 9: Managing Status..."
Add-ContentSlide "Step 6: Managing Student Status" "Two Options Available:`n`nSOFT DELETE (Recommended):`n- Marks student as inactive`n- Record remains in system`n- Data is preserved`n- Can be reactivated`n`nPERMANENT DELETE (Use with caution):`n- Completely removes record`n- Cannot be recovered`n- Requires confirmation"

# Slide 10
Write-Host "  Slide 10: Image Management..."
Add-ContentSlide "Step 7: Profile Image Management" "Image Upload Guidelines:`n`nSupported: JPEG (.jpg, .jpeg) and PNG (.png)`nSize Limit: Maximum 5 MB per image`nValidation: File signature verification`nProcessing: Images resized for efficiency`n`nUpload Process:`n- Click Choose Image or drag-and-drop`n- System validates format and integrity`n- Image uploaded to secure cloud storage"

# Slide 11
Write-Host "  Slide 11: Security..."
Add-ContentSlide "Security Features" "Your Data is Protected By:`n`n- Google OAuth 2.0 (Industry standard)`n- Admin Whitelist (Approved users only)`n- HTTPS Encryption (Transit protection)`n- Anti-CSRF Tokens (Attack protection)`n- Security Headers (HSTS, CSP)`n- SAS Links (15-minute image expiry)`n- Azure Cosmos DB (Enterprise protection)`n- Azure Blob Storage (Secure media)"

# Slide 12
Write-Host "  Slide 12: Best Practices..."
Add-ContentSlide "Best Practices & Tips" "Usage Guidelines:`n`n- Security: Never share login credentials`n- Data Quality: Enter accurate information`n- Backups: System auto-backs up`n- Soft Delete First: Use soft delete by default`n- Verify Updates: Confirm changes saved`n- Support: Contact IT for technical issues`n- Audit Trail: Use your own account only"

# Slide 13
Write-Host "  Slide 13: Troubleshooting..."
Add-ContentSlide "Troubleshooting Guide" "Common Issues:`n`nCannot Login:`n- Verify account registered with admin`n- Check email domain is whitelisted`n- Clear browser cookies`n`nImage Upload Failed:`n- Check JPEG or PNG format`n- Verify file size (max 5MB)`n- Ensure file not corrupted`n`nRecord Not Found:`n- Try different search terms`n- Check if marked as inactive"

# Slide 14
Write-Host "  Slide 14: Summary..."
Add-ContentSlide "You're Ready to Use FutureTech SMS!" "Key Capabilities:`n`n- Secure login with Google OAuth`n- Full student record management (CRUD)`n- Profile images with secure access`n- Search and pagination support`n- Enterprise-grade cloud infrastructure`n`nNeed Help?`nContact your system administrator`n`nFutureTech Academy Student Management System | v1.0"

# Save
$outputPath = "d:\FutureTech_Academy\StudentManagementSystem\docs\FutureTech-UserManual.pptx"
Write-Host "`nSaving presentation..."
$presentation.SaveAs($outputPath, 1)

# Cleanup
$presentation.Close()
$ppt.Quit()
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($ppt) | Out-Null

# Verify
if (Test-Path $outputPath) {
    $file = Get-Item $outputPath
    Write-Host "`n[SUCCESS] PowerPoint created!"
    Write-Host "File: $($file.FullName)"
    Write-Host "Size: $([math]::Round($file.Length/1MB,2)) MB"
    Write-Host "Date: $($file.LastWriteTime)"
} else {
    Write-Host "[ERROR] File not created"
}
