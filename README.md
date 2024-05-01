# ChatterV2

ChatterV2 is a chatting application built with .NET MAUI and .NET 8, designed specifically for Android. It provides end-to-end encrypted messaging using RSA and AES, ensuring that your conversations remain private and secure.

## Features

- **Account Management**: Register, login, and manage your profile with ease.
- **Friend System**: Invite, accept invitations, and search for friends to chat.
- **Messaging**: Send and receive messages securely with RSA and AES encryption. Support for text and image messages.
- **Local Storage**: Messages are securely stored locally using SQLite.
- **Notifications**: Background service to notify users of new messages.
- **User Profiles**: Profile pictures are dynamically selected based on the username hash.

## Technologies

- **Client-Side**: .NET MAUI and .NET 8 for Android app development.
- **Encryption**: RSA for key exchange and AES for message encryption.
- **Local Database**: SQLite for storing messages and other local data.
- **Server-Side**: API developed in .NET 8 ASP.NET Core Web API.
- **Data Storage**: Entity Framework with SQLite for data persistence.
- **Authentication**: JWT Bearer tokens for secure authorization.
- **Password Security**: Hashing with built-in IPasswordHasher.
