# Car Renting Company – Dynamics 365 Solution

This repository contains a custom Dynamics 365 implementation for a car rental company (Sevent). The solution covers CRM configuration, business logic plugins, client-side scripts, and practical tasks related to car reservations, rentals, handovers, and returns.

## Repository Structure

```
/practical-tasks     # Practical assignments, sample data generators, and related scripts
/scripts             # JavaScript client-side scripts for Dynamics 365 forms
/solutions           # Solution files for importing into Dynamics 365 (customizations, entities, dashboards)
/src                 # Source code: plugins and backend logic (C#)
```

## Contents

### 1. Plugins (C#)

The `/src` folder contains C# plugins implementing business logic for car rental operations:

* Status transition validation
* Price calculation
* Field validation and required fields
* Car availability filter (no more than 10 active rents per customer)
* Pickup/Return report automation and damage handling

### 2. Client-Side Scripts

The `/scripts` folder includes JavaScript files that control Dynamics 365 form behavior:

* Filtering cars by car class
* Automatic price and days calculation
* Date and field validation
* Status-dependent required fields and notifications
* Quick creation of pickup/return reports

### 3. Solutions

The `/solutions` folder contains solution files for importing customizations, entities, views, forms, and dashboards into Dynamics 365.

### 4. Practical Tasks

The `/practical-tasks` folder provides additional practical assignments, such as sample unit tests, or utilities used during development and testing.

### 5. Documentation

Business requirements and project details can be found in the provided PDF files (e.g., `Car_Renting_Company_Part1.pdf`, `3 Block.pdf`), describing:

* The data model and required entities (Car Class, Car, Rent, Car Transfer Report, Customer)
* Key user stories (car rent creation, handover, return)

---

## How to Use

1. **Import the Solution:**
   Import files from the `/solutions` folder into your Dynamics 365 environment.

2. **Deploy Plugins:**
   Build and register plugins from `/src` using Plugin Registration Tool.

3. **Add Web Resources:**
   Upload scripts from `/scripts` and add them to the relevant entity forms.

4. **Try Practical Assignments:**
   Explore `/practical-tasks` for sample data generators and development/test utilities.

---

## Author

Liubov Opryshchenko
Open to work as Junior Dynamics 365 Developer

