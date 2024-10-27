# schoolMoney_backend

Work in progress!

## Database

### Tables

#### Users
| Field           | Type   | Description                     |
|-----------------|--------|---------------------------------|
| UserId          | PK     | Unique identifier for the user  |
| Email           | String | User's email address           |
| PasswordHash    | String | Encrypted password             |
| CreatedAt       | Date   | Registration date              |

#### Classes
| Field           | Type   | Description                                     |
|-----------------|--------|-------------------------------------------------|
| ClassId         | PK     | Unique identifier for the class                 |
| Name            | String | Name of the class                               |
| SchoolName      | String | Name of the school                              |
| TreasurerId     | FK     | ID of the user serving as class treasurer       |

#### Children
| Field           | Type   | Description                                           |
|-----------------|--------|-------------------------------------------------------|
| ChildId         | PK     | Unique identifier for the child                        |
| Name            | String | Child's name                                          |
| ParentId        | FK     | ID of the parent (user)                               |
| ClassId         | FK     | ID of the class the child is assigned to              |

#### Fundraisers
| Field           | Type   | Description                                    |
|-----------------|--------|------------------------------------------------|
| FundraiserId    | PK     | Unique identifier for the fundraiser           |
| Title           | String | Title of the fundraiser                        |
| Description     | Text   | Description of the fundraiser                  |
| GoalAmount      | Float  | Financial goal for the fundraiser              |
| StartDate       | Date   | Start date of the fundraiser                   |
| EndDate         | Date   | End date of the fundraiser                     |
| ClassId         | FK     | ID of the class associated with the fundraiser |

#### Transactions
| Field               | Type   | Description                                       |
|---------------------|--------|---------------------------------------------------|
| TransactionId       | PK     | Unique identifier for the transaction             |
| FundraiserId        | FK     | ID of the fundraiser associated with the transaction |
| UserId              | FK     | ID of the parent (user) making the transaction    |
| Amount              | Float  | Amount of the transaction                         |
| Date                | Date   | Date of the transaction                           |
| Status              | Enum   | Status of the transaction (e.g., Deposit, Withdrawal) |
| VirtualAccountNumber| String | Virtual account number associated with the user   |

### Relationships

- **Users** and **Classes**: `TreasurerId` in Classes references `UserId` in Users (a parent creating a class can act as a treasurer).
- **Users** and **Children**: `ParentId` in Children references `UserId` in Users (a parent can have multiple children).
- **Classes** and **Children**: `ClassId` in Children references `ClassId` in Classes (each child is assigned to one class).
- **Classes** and **Fundraisers**: `ClassId` in Fundraisers references `ClassId` in Classes (a class can have multiple fundraisers).
- **Fundraisers** and **Transactions**: `FundraiserId` in Transactions references `FundraiserId` in Fundraisers (each transaction is linked to a specific fundraiser).
- **Users** and **Transactions**: `UserId` in Transactions references `UserId` in Users (a parent makes the transaction).



### Database Diagram

The following is a high-level schema diagram of the database structure, showing table relationships:

- **Users** ⟶ **Classes** (TreasurerId → UserId)
- **Users** ⟶ **Children** (ParentId → UserId)
- **Classes** ⟶ **Children** (ClassId → ClassId)
- **Classes** ⟶ **Fundraisers** (ClassId → ClassId)
- **Fundraisers** ⟶ **Transactions** (FundraiserId → FundraiserId)
- **Users** ⟶ **Transactions** (UserId → UserId)

This notation shows which table fields are foreign keys pointing to other tables.