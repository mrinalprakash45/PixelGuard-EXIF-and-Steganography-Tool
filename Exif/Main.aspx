<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Exif.Main" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <title>EXIF and STEGANOGRAPHY Tool</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <style>
        .nav-tabs {
        border-bottom: 2px solid #dee2e6;
    }
    .nav-tabs .nav-link {
        color: #495057;
        background-color: #e9ecef;
        border: none;
        border-radius: .25rem;
        margin: 0 2px;
        transition: all 0.3s ease;
    }
    .nav-tabs .nav-link:hover {
        transform: scale(1.05);
    }
    .nav-tabs .nav-link.active {
        color: #fff;
        background-color: #007bff;
        border-color: #007bff;
    }
    .tab-content {
        background: #fff;
        padding: 20px;
        border-radius: .25rem;
        box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1);
    }
.styled-heading {
    font-family: 'Arial', sans-serif;
    font-size: 2.5em;
    color: #ffffff;
    background-image: linear-gradient(to right, #6DD5FA, #2980B9);
    -webkit-background-clip: text;
    background-clip: text;
    color: transparent;
    text-align: center;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
    margin-bottom: 20px;
    padding: 10px;
}

.container {
    max-width: 800px;
    margin: auto;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container text-center mt-5">
            <h1 class="styled-heading">PixelGuard: EXIF and Steganography Tool</h1>
        </div>
        <div class="container mt-5">
            <ul class="nav nav-tabs" id="myTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <a class="nav-link active" id="exif-tab" data-bs-toggle="tab" href="#exif" role="tab" aria-controls="exif" aria-selected="true">
                        <i class="fas fa-camera-retro"></i> EXIF
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link" id="steganography-tab" data-bs-toggle="tab" href="#steganography" role="tab" aria-controls="steganography" aria-selected="false">
                        <i class="fas fa-image"></i> STEGANOGRAPHY
                    </a>
                </li>
            </ul>
            <div class="tab-content" id="myTabContent">
                <div class="tab-pane fade show active p-4" id="exif" role="tabpanel" aria-labelledby="exif-tab">
                    <div class="mb-3">
            <label for="fileUpload" class="form-label">Upload Image</label>
            <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" />
        </div>
        
        <div class="mb-3">
            <asp:Button ID="btnUpload" runat="server" Text="View Metadata" OnClick="btnUpload_Click" CssClass="btn btn-primary" />
        </div>
        
        <div class="mb-3">
            <asp:Label ID="lblMetadata" runat="server" CssClass="result-label" />
        </div>
                </div>
                <div class="tab-pane fade p-4" id="steganography" role="tabpanel" aria-labelledby="steganography-tab">
                    <div class="mb-3">
            <label for="fileUpload" class="form-label">Upload Image</label>
            <asp:FileUpload ID="fileUpload1" runat="server" CssClass="form-control" />
        </div>
        
        
        
        <div class="mb-3 form-check">
            <asp:CheckBox ID="chkSteganography" runat="server" CssClass="form-check-input" />
            <label class="form-check-label" for="chkSteganography">Perform Steganography</label>
        </div>
        
        <div class="mb-3">
            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" Placeholder="Enter message to encode" />
        </div>
        
        
        
        <div class="mb-3">
            <asp:Button ID="btnEncode" runat="server" Text="Encode Message" OnClick="btnEncode_Click" CssClass="btn btn-success" />
        </div>
        
        <div class="mb-3">
            <asp:Button ID="btnDecode" runat="server" Text="Decode Message" OnClick="btnDecode_Click" CssClass="btn btn-info" />
        </div>
        
        <div class="mb-3">
            <asp:Label ID="lblMessage" runat="server" CssClass="result-label" />
        </div>
                </div>
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.10.2/dist/umd/popper.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.min.js"></script>
</body>
</html>

