# Imagify

Alter 2D SVG dimensions and display the perimeter.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Frontend Setup](#frontend-setup)

## Features

- in the bordered rectangle, an SVG will appear after the user selects an existing SVG object through the dropdown
- user can alter SVG dimensions with their mouse
- Reset button resets the form
- Save on release updates the SVG dimensions in the json file on mouse button release
- Save on change punishes the server and DB by saving the SVG on frame change. Only gets the new dimensions when mouse is released
- Manual save activates Save SVG button and when clicked it saves to JSON
- Toggle version takes the object id (listId) and cycles through its versions
- Newest versions finds the object with the listId and gets first item in the versions array


## Technologies Used

- **Frontend:** React, Vite, TypeScript, react-router-dom, axios, MUI
- **Backend:** .NET, ASP.NET Core, WebSockets
- **Database:** JSON file storage

## Getting Started

### Frontend Setup

Dependencies:
npm install

Start:
npm start
