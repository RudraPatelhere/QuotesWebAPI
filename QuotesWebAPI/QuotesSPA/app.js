const apiUrl = "/api/quotes";
const tagApiUrl = "/api/tags";

async function loadQuotes() {
    try {
        const res = await fetch(apiUrl);
        const data = await res.json();
        const container = document.getElementById("quotes-container");
        container.innerHTML = "";

        data.forEach(q => {
            const div = document.createElement("div");
            div.className = "card mb-3";
            div.innerHTML = `
        <div class="card-body">
          <blockquote class="blockquote mb-0">
            <p>${q.text}</p>
            <footer class="blockquote-footer">${q.author || "Unknown"}</footer>
            <div class="d-flex justify-content-between mt-2">
              <span class="badge bg-secondary">Likes: ${q.likes}</span>
              <button class="btn btn-sm btn-outline-primary" onclick="likeQuote(${q.id})">👍 Like</button>
            </div>
            <div class="mt-2">
              ${q.tagAssignments.map(t => `<span class="tag">${t.tag.name}</span>`).join("")}
            </div>
          </blockquote>
        </div>
      `;
            container.appendChild(div);
        });
    } catch (err) {
        console.error("Error loading quotes:", err);
    }
}

async function loadTopQuotes() {
    try {
        const res = await fetch(`${apiUrl}/top?count=10`);
        const data = await res.json();
        const container = document.getElementById("top-quotes-container");
        container.innerHTML = "";

        data.forEach(q => {
            const div = document.createElement("div");
            div.className = "mb-3 border-bottom pb-2";
            div.innerHTML = `
        <blockquote class="blockquote">
          <p>${q.text}</p>
          <footer class="blockquote-footer">${q.author || "Unknown"} - Likes: ${q.likes}</footer>
        </blockquote>
      `;
            container.appendChild(div);
        });
    } catch (err) {
        console.error("Error loading top quotes:", err);
    }
}

async function likeQuote(id) {
    const res = await fetch(`${apiUrl}/${id}/like`, { method: "POST" });
    if (res.ok) {
        loadQuotes();
        loadTopQuotes();
    }
}

document.getElementById("quote-form").addEventListener("submit", async function (e) {
    e.preventDefault();
    const text = document.getElementById("quoteText").value;
    const author = document.getElementById("quoteAuthor").value;
    const tag = document.getElementById("quoteTag").value.trim();

    const res = await fetch(apiUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text, author })
    });

    if (res.ok) {
        const newQuote = await res.json();
        if (tag) {
            await fetch(`${apiUrl}/${newQuote.id}/tags`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(tag)
            });
        }
        this.reset();
        loadQuotes();
        loadTopQuotes();
    } else {
        alert("Failed to add quote");
    }
});

document.getElementById("quoteTag").addEventListener("input", async function () {
    const input = this.value.toLowerCase();
    const suggestionsBox = document.getElementById("tagSuggestions");
    if (!input) {
        suggestionsBox.innerHTML = "";
        return;
    }

    try {
        const res = await fetch(tagApiUrl);
        const tags = await res.json();
        const matches = tags.filter(t => t.name.toLowerCase().startsWith(input));
        suggestionsBox.innerHTML = matches.map(t => `<div onclick="selectTag('${t.name}')">${t.name}</div>`).join("");
    } catch {
        suggestionsBox.innerHTML = "";
    }
});

function selectTag(tag) {
    document.getElementById("quoteTag").value = tag;
    document.getElementById("tagSuggestions").innerHTML = "";
}

// Initial load
loadQuotes();
loadTopQuotes();
