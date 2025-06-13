# Bamboocard

# Deployment Guide

This project uses Docker to facilitate seamless setup and deployment. With Docker Compose, both the SQL Server and ASP.NET Core web application are automatically configured and started together. The SQL Server container includes an initialization script that restores the database from a backup during startup.

Nop admin user : admin@yourstore.com
password : 123.123

NOT: discount plugin require 3 and more orders with payment status paid and order status is complated

## Requirements
- Docker and Docker Compose must be installed

## Setup and Launch

### 1. Clone the repository (if applicable)

bash git clone <your-repo-url> cd <your-repo-directory>


### 2. Launch all services
Run the following command to build and start all containers:


bash docker-compose up -d


This command will automatically set up the SQL Server and the web application. The SQL container will execute the `init/sh` script inside the `init` folder, which restores the database from your backup files.

## Inside the `init` Folder
The `init` folder contains the following script:


bash #!/bin/bash


# Start SQL Server
/opt/mssql/bin/sqlservr &
# Wait until SQL Server is ready
sleep 30
# Execute the restore script
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "6111098.Cc" -i /restore/restore.sql -N -C
# Keep the container running as long as SQL Server is active
wait


This script automatically restores the database when the container starts and keeps the container alive.

## Notes
- The volume mappings and paths in your `docker-compose.yml` ensure that the script and backup files are correctly mounted.
- The `init.sh` script should be configured to run automatically during container startup. This can be set up in your Dockerfile or `docker-compose.yml`.

## Additional Commands
- To stop the services:

bash docker-compose down


- To rebuild and restart everything:


bash docker-compose build docker-compose up -d



By following these steps, your SQL Server and web application will be automatically deployed, with the database restored from your backup, ready for use.

---


## About the Included Plugins

The project includes a pre-installed **nopCommerce plugin** named:

**`Nop.Plugin.DiscountRules.OrderHistory`**

This plugin offers discounts for customers who have placed more than 3 orders. Details:

- **Use Case:** Discount for customers with 3 or more orders.
- **Functionality:**  
  After installation, you can configure the settings by clicking the **Configure** button in the plugin list within the nopCommerce admin panel. The configurable options are:

  - **isActive**  
    Enables or disables the discount rule.

  - **Minimum Orders (`discountfororders`)**  
    The discount will be applied if the customer's total previous orders are **greater than or equal to** this value.

  - **Discount Percentage (`discountpercentage`)**  
    The percentage of discount to be applied.

### Plugin Deployment
If you'd like to use this plugin on a different nopCommerce project, simply copy the `Nop.Plugin.DiscountRules.OrderHistory` folder into the **`Plugins`** directory of that project and run the application.

---

## Additional Features

### Product Attribute Search Enhancements
Under the **Catalog** menu, a new optional feature has been added to the **Product Attributes** section, allowing users to search products by name more efficiently.

---

## Postman API Testing

Below are the steps to test the API securely using Postman with JWT authentication:

### 1. Obtain JWT Token (Login)
- **Request URL:**

[http://localhost:8012/api/auth/login/](http://localhost:8012/api/auth/login/)

- **Method:** `POST`

- **Request Body:**

json { "email": "admin@yourstore.com", "password": "123.123" }


- **Response:**  
  You will receive a JWT token, e.g.,

json { "token": "YOUR_JWT_TOKEN_HERE", "expires": "2025-07-01T12:34:56Z" }

Save this token; you'll need it in the next step.

---

### 2. Retrieve Orders by Email
- **Request URL:**
- 
[http://localhost:8012/api/orders/by-email?email=admin@yourstore.com](http://localhost:8012/api/orders/by-email?email=admin@yourstore.com)

- **Method:** `GET`

- **Headers:**  
  - Key: `Authorization`  
  - Value: `Bearer YOUR_JWT_TOKEN_HERE`

Replace `YOUR_JWT_TOKEN_HERE` with the token you obtained in step 1.

---

### Using Postman
1. **Login Request:**  
   - URL: `http://localhost:8012/api/auth/login/`  
   - Method: `POST`  
   - Body: Use the JSON provided above to log in and get the token.

2. **Order Retrieval Request:**  
   - URL: `http://localhost:8012/api/orders/by-email?email=admin@yourstore.com`  
   - Method: `GET`  
   - Headers:  
     - Key: `Authorization`  
     - Value: `Bearer <your-token>`

   Replace `<your-token>` with the token obtained in step 1.  
   Send the request to see the order details.


Finally, the Postman collection file for testing the API is located in the root folder, named **`Bamboocard_API_Collection.json`**.
 
---




