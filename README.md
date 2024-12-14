# Library Management System
## Overview
The Library Management System is an application that can be used to to manage library operations like book management, member registration, and book returns
## Features 
### Book Management
- Add new books to the database
- Delete books
- List all available books
### Member Management
- Register new members
- Display a list of all members
### Borrowing and Returning
- Borrow books for members
- Track checkout details
- Search for checkout record based on member or books
- Return books and update status
### Book Reservation
- Members can reserve books for later
- Search for reservations based on member or books
- Cancel Reservation

## How to Run
  1. Clone Repo
  2. Setup Database
     - Connect to local database and change connection string in Data/DbManager.cs
     - Create required tables by running sql script
  3. Run the application
