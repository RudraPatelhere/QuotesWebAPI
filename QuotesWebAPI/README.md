# Quotes Management Project

A full-stack application for creating, managing, and viewing quotes. Built with an ASP.NET Core Web API, a single-page front-end using HTML/CSS/JS, and a Python command-line client.

---

## Features

1. **ASP.NET Core Web API**  
   - Create, read, update, and delete quotes  
   - Like quotes  
   - Tag quotes with custom labels  
   - Retrieve top-liked quotes  

2. **Responsive Front-End (SPA)**  
   - Built with HTML, Bootstrap, and custom CSS  
   - Add quotes with an optional author and tag  
   - Autocomplete for existing tags  
   - Filter quotes by tag  
   - Display top-liked quotes  

3. **Python CLI Client**  
   - Load quotes from a file  
   - Add new quotes from the console  
   - Display a random quote  

---

## Getting Started

1. **Clone or download** this repository  
2. **Database & Migrations**  
   - Open Package Manager Console  
   - Run `Update-Database` to create/update the database  
3. **Run Web API**  
   - Press F5 in Visual Studio, or run `dotnet run`  
   - Confirm it listens on `https://localhost:7149`  
4. **View the SPA**  
   - Open `https://localhost:7149/index.html` in your browser  
5. **Python Client**  
   - Navigate to the `PythonClient` folder  
   - Run `python client.py` (ensure `pip install requests` if needed)  

---

## Screenshots

**1. Front-End SPA**  
![Quotes SPA Screenshot](QuotesWebAPI/assets/SS_1.png)

Displays the grid of quotes, plus a form to add new ones and a top-liked section.

<br/>

**2. Python CLI**  
![Python CLI Screenshot](QuotesWebAPI/assets/SS_2.png)

Shows the console-based menu for loading quotes, adding new ones, and displaying random quotes.

---

## Project Structure

