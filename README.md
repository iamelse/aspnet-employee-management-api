# Employee Management API

## Deskripsi

Aplikasi backend sederhana berbasis ASP.NET Core yang berfungsi sebagai Employee Management System.  
Aplikasi ini menggunakan SQLite/MySQL sebagai database dan menerapkan enkripsi untuk data sensitif seperti tanggal lahir karyawan.  
API ini menyediakan endpoint CRUD untuk entitas Employee dan JobPosition, mendukung pengelolaan data karyawan dan posisi pekerjaan secara efisien.

---

## Fitur Utama

- CRUD untuk Employee dan Job Position
- Enkripsi data sensitif (Date of Birth)
- Seeder & Factories untuk dummy data
- Validasi bisnis: hanya 1 posisi kerja aktif per karyawan
- RESTful API dengan format JSON
- Dokumentasi API menggunakan Swagger/OpenAPI
- Menggunakan SQLite/MySQL sebagai database ringan

---

## Teknologi yang Digunakan

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core (ORM)
- SQLite/MySQL sebagai database
- Swagger untuk dokumentasi API
- Dependency Injection dan Service Layer (EncryptionService)

---

## Struktur Folder

```bash
EmployeeManagementAPI/
├── Controllers/
│ └── EmployeesController.cs
├── Models/
│ ├── Employee.cs
│ └── JobPosition.cs
├── Data/
│ └── AppDbContext.cs
│ └── DatabaseSeeder.cs
├── Services/
│ └── EncryptionService.cs
├── Settings/
│ └── EncryptionSettings.cs
├── DTOs/
│ ├── EmployeeDTO.cs
│ └── JobPositionDTO.cs
├── Factories/
│ ├── EmployeeFactory.cs
│ └── JobPositionFactory.cs
├── Program.cs
├── appsettings.json
├── README.md
```

---

## Persiapan & Instalasi

### Prasyarat

- .NET SDK 7.x atau lebih tinggi
- Visual Studio 2022 / VS Code / IDE lain yang support .NET

### Langkah Setup

1. **Clone repository**

```bash
git https://github.com/iamelse/aspnet-employee-management-api.git
cd aspnet-employee-management-api
```

Restore NuGet packages
```bash
dotnet restore
```

Update Database dengan Migration

Pastikan sudah membuat migration awal (jika belum):

### Buat migration awal (sekali)
```
dotnet ef migrations add InitialCreate
```

### Update database SQLite
### Pastikan "Provider" di appsettings.json = "SQLite"
```
dotnet ef database update
```

### Update database MySQL
### Pastikan "Provider" di appsettings.json = "MySQL"
```
dotnet ef database update
```
    
### Jalankan Aplikasi
```
dotnet run
```

### Route List Penggunaan API
```
GET /api/employees : Mendapatkan list semua karyawan

POST /api/employees : Menambahkan karyawan baru

PUT /api/employees/{id} : Update data karyawan

DELETE /api/employees/{id} : Hapus karyawan
```

### Contoh JSON body untuk POST dan PUT

```bash
{
  "firstName": "Budi",
  "middleName": "Santoso",
  "lastName": "Wijaya",
  "dateOfBirth": "1990-05-10T00:00:00",
  "gender": "Male",
  "address": "Jl. Merdeka No.1",
  "jobPositions": [
    {
      "jobName": "Software Engineer",
      "startDate": "2023-01-01T00:00:00",
      "endDate": null,
      "salary": 15000000,
      "status": "active"
    }
  ]
}
```

### Testing

Untuk unit test ada di branch test

```
dotnet test
```

### FrontEnd

https://github.com/iamelse/employee-management