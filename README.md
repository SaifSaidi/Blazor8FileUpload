# Blazor8FileUpload - File Upload Manager

## Overview

Blazor8FileUpload is a file upload service designed for Blazor applications. It provides robust features for handling file uploads, including validation, file storage organization, and file removal. This service is capable of managing different file types (e.g., images, documents, videos) and ensures compliance with specific validation rules such as file size and extensions.

## Features

- **File Upload from Browser**: Supports file uploads through `IBrowserFile` and `IFormFile`.
- **File Validation**: Ensures valid file extensions and size limits based on the file category (e.g., images, documents, videos).
- **Custom Directory Structure**: Uploaded files are organized into folders based on file type and custom subdirectories.
- **File Deletion**: Removes files from the server with proper error handling and logging.
- **Async File Handling**: Uses asynchronous methods for file uploads to ensure non-blocking operations.

## Installation

1. Clone or download the Blazor8FileUpload service.

## Usage

### Inject the `FileUploadManager` Service

In your Blazor component, inject the `FileUploadManager` service to manage file uploads.

```csharp
@inject FileUploadManager _fileUploadManager
