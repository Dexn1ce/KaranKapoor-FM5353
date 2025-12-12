## Database Setup (Using SQL Files)

1. Create the database:

   ```bash
   createdb -U postgres finaldb
```

2. Apply Schema

```bash
   psql -U postgres -d finaldb -f sql/schema.sql
```

3. Load data:
```bash 
   psql -U postgres -d finaldb -f sql/seed.sql
```

4. Update connections string in OptionDbcontext in the controllers director
"Host=localhost;Port=5432;Database=finaldb;Username=postgres;Password=yourpassword"

5. Run the API
   dotnet run
