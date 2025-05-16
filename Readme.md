
## APBD_3 Task 8

## API Endpoints

### 1. `GET /api/trips`
Returns all available trips and their all countries.

### 2. `GET /api/clients/{id}/trips`
Returns all trips a client is registered to, including countries and registration info.

### 3. `POST /api/clients`
Creates a new client. Requires: `FirstName`, `LastName`, `Email`, `Telephone`, `Pesel`.

### 4. `PUT /api/clients/{id}/trips/{tripId}`
Registers a client to a trip. Fails if client/trip doesn't exist, trip is full, or client is already registered.

### 5. `DELETE /api/clients/{id}/trips/{tripId}`
Removes a client's registration from a trip. Returns the removed registration info if it existed.

## Connect to Database via connection String
```
Server=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False
``` 

## How to Run the Project

1. Clone the repo:
   ```bash
   git clone https://github.com/x0-lf/APBD_3.git

2. Navigate to the Project Dir:
    ```bash
    cd APBD_3
3. ```bash
    dotnet run
   
## How to send requests from JetBrains Rider

The request are well written however they might need some modifications.
For such Look at the root folder of the directory at the file
`APBD_3.http` there you can easily modify requests.

1. Open the `.http` file.
2. Each HTTP block is automatically detected.
3. Click the green **=> Run** button to test.

Each request is pre-configured for your local server at `http://localhost:5000`
You can easily change it at the same file by modifying the `APBD_3_HostAddress` variable. 

## Example Positive Responses

### 1. `GET /api/trips`

```
[
  {
    "idTrip": 1,
    "name": "Mediterranean Cruise",
    "description": "Explore the beautiful Mediterranean coast",
    "dateFrom": "2025-06-15T00:00:00",
    "dateTo": "2025-06-25T00:00:00",
    "maxPeople": 100,
    "tripCountries": [
      {
        "idCountry": 1,
        "name": "Spain"
      },
      {
        "idCountry": 2,
        "name": "Italy"
      },
      {
        "idCountry": 3,
        "name": "France"
      }
    ]
  },
  ...
```

### 2. `GET /api/clients/{id}/trips`
```
[
  {
    "idTrip": 1,
    "name": "Mediterranean Cruise",
    "description": "Explore the beautiful Mediterranean coast",
    "dateFrom": "2025-06-15T00:00:00",
    "dateTo": "2025-06-25T00:00:00",
    "maxPeople": 100,
    "registeredAt": 20250501,
    "paymentDate": 20250505,
    "countries": [
      {
        "idCountry": 1,
        "name": "Spain"
      },
      {
        "idCountry": 2,
        "name": "Italy"
      },
      {
        "idCountry": 3,
        "name": "France"
      }
    ]
  },
  ...
```

### 3. `POST /api/clients`
```
{
  "id": 1004
}
```
### 4. `PUT /api/clients/{id}/trips/{tripId}`
```
{
  "message": "Client successfully registered.",
  "data": {
    "clientId": 4,
    "tripId": 4,
    "registeredAt": 1747170742
  }
}
```
### 5. `DELETE /api/clients/{id}/trips/{tripId}`
```
{
  "message": "Client 1 was successfully unregistered from trip 2.",
  "deleted": {
    "clientId": 1,
    "tripId": 2,
    "registeredAt": 1747160627,
    "paymentDate": null
  }
}
```

**[APBD_4 – Warehouse Management](https://github.com/x0-lf/APBD_4)**  
A more advanced yet similar to this project, implementing stored procedures, better layered architecture.
