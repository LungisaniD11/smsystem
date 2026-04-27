$ErrorActionPreference = 'Stop'

# Create PowerPoint COM object
$ppt = New-Object -ComObject PowerPoint.Application
$ppt.Visible = -1

# Add blank presentation
$presentation = $ppt.Presentations.Add()

# Create slides using different approach
Write-Host "Creating presentation with 14 slides..."

# Slide layouts: 1=Blank
$blankLayout = 6

# Helper to add text
function Add-Slide-With-Text {
    param([object]$pres, [string]$title, [string[]]$content)
    
    $slide = $pres.Slides.Add($pres.Slides.Count + 1, $blankLayout)
    
    # Add title text box - Left(50), Top(30), Width(860), Height(60)
    $shape = $slide.Shapes.AddShape(1, 50, 30, 860, 60)
    $shape.TextFrame.Text = $title
    $shape.TextFrame.Paragraphs(1).Font.Size = 44
    $shape.TextFrame.Paragraphs(1).Font.Bold = $true
    $shape.TextFrame.Paragraphs(1).Font.Color.RGB = 14320750  # Teal RGB(15,118,110)
    $shape.Fill.Solid()
    $shape.Fill.ForeColor.RGB = 16777215  # White
    $shape.Line.ColorFormat.RGB = 14320750
    
    # Add content
    $contentText = $content -join "`n"
    $shape = $slide.Shapes.AddShape(1, 50, 100, 860, 380)
    $shape.TextFrame.Text = $contentText
    $shape.TextFrame.Paragraphs(1).Font.Size = 16
    $shape.TextFrame.Paragraphs(1).Font.Color.RGB = 3355443  # Dark gray
    $shape.Fill.Solid()
    $shape.Fill.ForeColor.RGB = 16777215
    $shape.Line.ColorFormat.RGB = 13421772
}

# Slide 1 - Title
Write-Host "  Slide 1: Title..."
$slide = $presentation.Slides.Add(1, $blankLayout)

$shape = $slide.Shapes.AddShape(1, 50, 150, 860, 150)
$shape.TextFrame.Text = "FutureTech Academy"
$shape.TextFrame.Paragraphs(1).Font.Size = 54
$shape.TextFrame.Paragraphs(1).Font.Bold = $true
$shape.TextFrame.Paragraphs(1).Font.Color.RGB = 14320750
$shape.Fill.Solid()
$shape.Fill.ForeColor.RGB = 16777215
$shape.Line.ColorFormat.RGB = 14320750

$shape = $slide.Shapes.AddShape(1, 50, 320, 860, 150)
$shape.TextFrame.Text = "Student Management System`nUser Manual v1.0"
$shape.TextFrame.Paragraphs(1).Font.Size = 28
$shape.TextFrame.Paragraphs(1).Font.Color.RGB = 5263440  # Gray
$shape.Fill.Solid()
$shape.Fill.ForeColor.RGB = 16777215
$shape.Line.ColorFormat.RGB = 13421772

# Slide 2 - Overview
Write-Host "  Slide 2: Overview..."
Add-Slide-With-Text $presentation "System Overview" @(
    "Secure cloud-based platform for managing student records, profiles, and enrollment data.",
    "",
    "Key Features:",
    "- Comprehensive Dashboard",
    "- Google OAuth 2.0 Authentication",
    "- Complete CRUD Operations",
    "- Profile Image Management",
    "- Search and Pagination",
    "- Enterprise Azure Infrastructure"
)

# Slide 3 - Getting Started with QR
Write-Host "  Slide 3: Getting Started..."
$slide = $presentation.Slides.Add($presentation.Slides.Count + 1, $blankLayout)

$shape = $slide.Shapes.AddShape(1, 50, 30, 860, 60)
$shape.TextFrame.Text = "Getting Started: Quick Access"
$shape.TextFrame.Paragraphs(1).Font.Size = 44
$shape.TextFrame.Paragraphs(1).Font.Bold = $true
$shape.TextFrame.Paragraphs(1).Font.Color.RGB = 14320750
$shape.Fill.Solid()
$shape.Fill.ForeColor.RGB = 16777215
$shape.Line.ColorFormat.RGB = 14320750

$qrPath = "d:\FutureTech_Academy\StudentManagementSystem\futuretech_qr.png"
if (Test-Path $qrPath) {
    $slide.Shapes.AddPicture($qrPath, $false, $true, 350, 120, 250, 250)
    Write-Host "    QR code added"
}

$shape = $slide.Shapes.AddShape(1, 50, 380, 860, 120)
$shape.TextFrame.Text = "Scan QR code to access the system`n`nhttps://futuretechsmsld-fnfgf9evhpghgzbv.southafricanorth-01.azurewebsites.net/"
$shape.TextFrame.Paragraphs(1).Font.Size = 14
$shape.Fill.Solid()
$shape.Fill.ForeColor.RGB = 16777215
$shape.Line.ColorFormat.RGB = 13421772

# Slides 4-14 - Content slides
Write-Host "  Slide 4: Authentication..."
Add-Slide-With-Text $presentation "Step 1: User Authentication" @(
    "Login Steps:",
    "",
    "1. Open application via QR code or URL",
    "2. Click Login with Google",
    "3. Select your Google account",
    "4. Grant permissions",
    "5. Redirected to dashboard if authorized",
    "",
    "Note: Only registered admin emails can access."
)

Write-Host "  Slide 5: Dashboard..."
Add-Slide-With-Text $presentation "Step 2: Dashboard Overview" @(
    "Dashboard Features:",
    "",
    "- Active Students Count",
    "- Inactive Students Count",
    "- Students with Profile Images",
    "- New Students This Week",
    "",
    "Functionality:",
    "- Quick access to recent students",
    "- Search by name or student ID",
    "- Navigation menu for all functions"
)

Write-Host "  Slide 6: Adding Students..."
Add-Slide-With-Text $presentation "Step 3: Adding a New Student" @(
    "Create Student Record:",
    "",
    "1. Click Add New Student from dashboard",
    "2. Enter: First Name, Last Name, Email, Mobile",
    "3. Select Enrollment Status",
    "4. Upload profile image (JPEG/PNG, max 5MB, optional)",
    "5. Click Save",
    "",
    "Formats: JPEG and PNG only",
    "File signature verification prevents spoofing"
)

Write-Host "  Slide 7: Searching..."
Add-Slide-With-Text $presentation "Step 4: Searching & Viewing Students" @(
    "Search and View Records:",
    "",
    "1. Use search bar by Name or Student ID",
    "2. Results paginated (10 per page)",
    "3. Click student name for full details",
    "4. Profile images shown with secure links",
    "",
    "Security: Image links expire after 15 minutes",
    "Prevents unauthorized access to student profiles"
)

Write-Host "  Slide 8: Editing..."
Add-Slide-With-Text $presentation "Step 5: Editing Student Records" @(
    "Update Student Information:",
    "",
    "1. Find student via search or browse",
    "2. Click Edit button",
    "3. Update any field (names, email, phone, status)",
    "4. Replace profile image if needed",
    "5. Click Save Changes",
    "",
    "Tip: Leave image field empty to keep existing image"
)

Write-Host "  Slide 9: Managing Status..."
Add-Slide-With-Text $presentation "Step 6: Managing Student Status" @(
    "Two Options Available:",
    "",
    "SOFT DELETE (Recommended):",
    "- Marks student as inactive",
    "- Record remains in system",
    "- Data is preserved",
    "- Can be reactivated",
    "",
    "PERMANENT DELETE (Use with caution):",
    "- Completely removes record",
    "- Cannot be recovered",
    "- Requires confirmation"
)

Write-Host "  Slide 10: Image Management..."
Add-Slide-With-Text $presentation "Step 7: Profile Image Management" @(
    "Image Upload Guidelines:",
    "",
    "Supported: JPEG (.jpg, .jpeg) and PNG (.png)",
    "Size Limit: Maximum 5 MB per image",
    "Validation: File signature verification",
    "Processing: Images resized for efficiency",
    "",
    "Upload Process:",
    "- Click Choose Image or drag-and-drop",
    "- System validates format and integrity",
    "- Image uploaded to secure cloud storage"
)

Write-Host "  Slide 11: Security..."
Add-Slide-With-Text $presentation "Security Features" @(
    "Your Data is Protected By:",
    "",
    "- Google OAuth 2.0 (Industry standard)",
    "- Admin Whitelist (Approved users only)",
    "- HTTPS Encryption (Transit protection)",
    "- Anti-CSRF Tokens (Attack protection)",
    "- Security Headers (HSTS, CSP)",
    "- SAS Links (15-minute image expiry)",
    "- Azure Cosmos DB (Enterprise protection)",
    "- Azure Blob Storage (Secure media)"
)

Write-Host "  Slide 12: Best Practices..."
Add-Slide-With-Text $presentation "Best Practices & Tips" @(
    "Usage Guidelines:",
    "",
    "- Security: Never share login credentials",
    "- Data Quality: Enter accurate information",
    "- Backups: System auto-backs up",
    "- Soft Delete First: Use soft delete by default",
    "- Verify Updates: Confirm changes saved",
    "- Support: Contact IT for technical issues",
    "- Audit Trail: Use your own account only"
)

Write-Host "  Slide 13: Troubleshooting..."
Add-Slide-With-Text $presentation "Troubleshooting Guide" @(
    "Common Issues:",
    "",
    "Cannot Login:",
    "- Verify account registered with admin",
    "- Check email domain is whitelisted",
    "- Clear browser cookies",
    "",
    "Image Upload Failed:",
    "- Check JPEG or PNG format",
    "- Verify file size (max 5MB)",
    "- Ensure file not corrupted",
    "",
    "Record Not Found:",
    "- Try different search terms",
    "- Check if marked as inactive"
)

Write-Host "  Slide 14: Summary..."
Add-Slide-With-Text $presentation "You're Ready to Use FutureTech SMS!" @(
    "Key Capabilities:",
    "",
    "- Secure login with Google OAuth",
    "- Full student record management (CRUD)",
    "- Profile images with secure access",
    "- Search and pagination support",
    "- Enterprise-grade cloud infrastructure",
    "",
    "Need Help?",
    "Contact your system administrator",
    "",
    "FutureTech Academy Student Management System | v1.0"
)

# Save presentation
$outputPath = "d:\FutureTech_Academy\StudentManagementSystem\docs\FutureTech-UserManual.pptx"
Write-Host "`nSaving to: $outputPath"
$presentation.SaveAs($outputPath, 1)

# Close
$presentation.Close()
$ppt.Quit()
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($ppt) | Out-Null

# Verify
if (Test-Path $outputPath) {
    $file = Get-Item $outputPath
    Write-Host "`n[OK] PowerPoint presentation created successfully!"
    Write-Host "Size: $([math]::Round($file.Length/1MB,2)) MB"
    Write-Host "Date: $($file.LastWriteTime)"
} else {
    Write-Host "[ERROR] File not created"
    exit 1
}
