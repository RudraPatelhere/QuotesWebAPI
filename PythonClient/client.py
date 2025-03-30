import requests
import random
import urllib3

# Correct API URL (matches your running project)
API_URL = "https://localhost:7149/api/quotes"

# Disable SSL warnings for localhost development
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


def load_quotes_from_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        for line in file:
            if line.strip():
                parts = line.strip().split("~")  # Format: quote ~ author
                text = parts[0].strip()
                author = parts[1].strip() if len(parts) > 1 else None

                payload = {"text": text, "author": author}
                res = requests.post(API_URL, json=payload, verify=False)
                if res.status_code in [200, 201]:
                    print(f"✅ Added: {text[:50]}...")
                else:
                    print(f"❌ Failed: {text[:50]}...")


def add_new_quote():
    text = input("Enter quote: ")
    author = input("Enter author (optional): ")
    payload = {"text": text, "author": author}
    res = requests.post(API_URL, json=payload, verify=False)

    if res.ok:
        print("✅ Quote added successfully!")
    else:
        print("❌ Failed to add quote.")


def get_random_quote():
    res = requests.get(API_URL, verify=False)
    if res.ok:
        quotes = res.json()
        if quotes:
            q = random.choice(quotes)
            print(f'\n"{q["text"]}"')
            print(f'  – {q["author"] or "Unknown"} (Likes: {q["likes"]})\n')
        else:
            print("No quotes available.")
    else:
        print("❌ Failed to fetch quotes.")


def main():
    print("== Quote Manager CLI ==")
    print("1. Load quotes from file")
    print("2. Add a new quote")
    print("3. Display a random quote")
    choice = input("Select an option: ")

    if choice == "1":
        path = input("Enter file path: ")
        load_quotes_from_file(path)
    elif choice == "2":
        add_new_quote()
    elif choice == "3":
        get_random_quote()
    else:
        print("Invalid option")


if __name__ == "__main__":
    main()
