The **Cost Tracker App** is a web-based application designed to help teams manage and track project costs efficiently. The app includes:

- **Authentication System**: Secure user login and registration using token-based authentication.
- **Cost Tracking**: Users can input and track project-related costs.
- **Email Notifications**: Personalized emails are sent to all team members involved in the project, summarizing the costs and updates.
- **Token Generation**: Unique tokens are generated for each session or transaction to ensure secure communication.

This app is ideal for small to medium-sized teams,Organization tracking incured cost of cloud operations w projects where cost transparency and accountability are critical to make informed decisons.

---

## Features

### 1. **Authentication**
- User registration and login with secure password hashing.
- Token-based authentication for API requests.
- Session management to ensure secure access.

### 2. **Cost Tracking**
- View a summary of all costs associated with a project.
- Filter costs by date, category, or team member.

### 3. **Gmail Integration**
- Automatically sends personalized emails to all team members involved in the project.
- Email content includes:
  - Project name
  - Total costs incurred

### 4. **Token Generation**
- Generates unique tokens for each user session or transaction.
- Ensures secure communication between the client and server.

---

## Technologies Used

- **Backend**: Node.js
- **Database**: MongoDB 
- **MSAL Authentication**: JWT (JSON Web Tokens)
- **Email Service**: Gmail API (via Nodemailer or similar library)

---

## Installation

### Prerequisites

Before running the app, ensure you have the following installed on your system:

- Node.js (v16 or higher)
- MongoDB (local or cloud instance)
- Gmail API credentials (for email functionality)

### Steps to Set Up

1. **Clone the Repository**
   ```bash
   git clone https://github.com/gabrielfrancisa/AZURE-COST-TRACKER-app.git
   cd cost-tracker-app
   ```

2. **Install Dependencies**
   ```bash
   npm install
   ```

3. **Set Up Environment Variables**
   Create a `.env` file in the root directory and add the following variables:
   ```env
   PORT=5000
   MONGO_URI=your_mongodb_connection_string
   JWT_SECRET=your_jwt_secret_key
   GMAIL_USER=your_gmail_email
   GMAIL_PASS=your_gmail_password_or_app_specific_password
   ```

4. **Run the Application**
   Start the backend server:
   ```bash
   npm run server
   ```

   Start the frontend (if applicable):
   ```bash
   npm run client
   ```

5. **Access the App**
   Open your browser and navigate to `withRedirectUri "http://localhost":3000` 

---

## Usage

### 1. **User Registration and Login**
- Register a new Azure account via the `/register` endpoint or UI.
- Log in using your credentials to receive a JWT token.

### 3. **Receiving Emails**
- After display of cost on the console or updating costs, an email will be sent to all team members involved.
- The email will include a summary of the project's financial status.

---

## API Endpoints

| Method | Endpoint          | Description                          |
|--------|-------------------|--------------------------------------|
| POST   | `/api/auth/login` | Authenticate user and generate token |
| POST   | `/api/auth/register` | Register a new user                |
| GET    | `/api/costs`      | Retrieve all cost entries            |
| POST   | `/api/costs`      | Add a new cost entry                 |
| PUT    | `/api/costs/:id`  | Update an existing cost entry        |
| DELETE | `/api/costs/:id`  | Delete a cost entry                  |

---

## Contributing

We welcome contributions from the community! If you'd like to contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit your changes with clear and concise messages.
4. Push your branch to your forked repository.
5. Submit a pull request detailing your changes.


## Contact

For questions, feedback, or collaboration, feel free to reach out:

- **Email**: gabrielfrancisa@gmail.com


Thank you for checking out the Cost Tracker App! 
We hope it helps streamline your project cost management process. 😊

Let me know if you'd like to customize this further!
