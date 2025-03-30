const apiUrl = "https://localhost:7149/api/quotes";
const tagApiUrl = "https://localhost:7149/api/tags";

// Load all quotes
async function loadQuotes() {
    const container = document.getElementById("quotes-container");
    container.innerHTML = "<p class='text-muted text-center'>Loading quotes...</p>";

    try {
        const res = await fetch(apiUrl);
        const data = await res.json();
        container.innerHTML = "";

        data.forEach(q => {
            const col = document.createElement("div");
            col.className = "col-md-6 col-lg-4";

            const tags = (q.tagAssignments || [])
                .map(t => `<span class="tag" onclick="filterByTag('${t.tag.name}')">${t.tag.name}</span>`)
                .join("");

            col.innerHTML = `
                <div class="card h-100">
                    <div class="card-body">
                        <blockquote class="blockquote mb-0">
                            <p id="quote-text-${q.id}">${q.text}</p>
                            <footer class="blockquote-footer" id="quote-author-${q.id}">${q.author || "Unknown"}</footer>
                            <div class="d-flex justify-content-between mt-2">
                                <span class="badge bg-secondary">Likes: ${q.likes}</span>
                                <div>
                                    <button class="btn btn-sm btn-outline-primary me-1" onclick="likeQuote(${q.id})">Like</button>
                                    <button class="btn btn-sm btn-outline-warning me-1" onclick="enableEdit(${q.id})">Edit</button>
                                    <button class="btn btn-sm btn-outline-danger" onclick="deleteQuote(${q.id})">Delete</button>
                                </div>
                            </div>
                            <div class="mt-2">${tags}</div>
                        </blockquote>
                    </div>
                </div>
            `;

            container.appendChild(col);
        });
    } catch (err) {
        console.error("Error loading quotes:", err);
        container.innerHTML = "<p class='text-danger text-center'>Failed to load quotes.</p>";
    }
}

// Load top liked quotes
async function loadTopQuotes() {
    const container = document.getElementById("top-quotes-container");
    container.innerHTML = "<p class='text-muted'>Loading top quotes...</p>";

    try {
        const res = await fetch(`${apiUrl}/top?count=10`);
        const data = await res.json();
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
        container.innerHTML = "<p class='text-danger'>Failed to load top quotes.</p>";
    }
}

// Like quote
async function likeQuote(id) {
    const res = await fetch(`${apiUrl}/${id}/like`, { method: "POST" });
    if (res.ok) {
        loadQuotes();
        loadTopQuotes();
    }
}

// Edit mode
function enableEdit(id) {
    const textEl = document.getElementById(`quote-text-${id}`);
    const authorEl = document.getElementById(`quote-author-${id}`);

    const currentText = textEl.innerText;
    const currentAuthor = authorEl.innerText;

    textEl.innerHTML = `<input type="text" class="form-control mb-2" id="edit-text-${id}" value="${currentText}" />`;
    authorEl.innerHTML = `
        <input type="text" class="form-control" id="edit-author-${id}" value="${currentAuthor === "Unknown" ? "" : currentAuthor}" />
        <button class="btn btn-sm btn-success mt-2" onclick="saveEdit(${id})">Save</button>`;
}

// Save edited quote
async function saveEdit(id) {
    const newText = document.getElementById(`edit-text-${id}`).value;
    const newAuthor = document.getElementById(`edit-author-${id}`).value;

    const res = await fetch(`${apiUrl}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id, text: newText, author: newAuthor })
    });

    if (res.ok) {
        loadQuotes();
        loadTopQuotes();
    } else {
        alert("Failed to update quote.");
    }
}

// Delete quote
async function deleteQuote(id) {
    if (!confirm("Are you sure you want to delete this quote?")) return;

    const res = await fetch(`${apiUrl}/${id}`, { method: "DELETE" });

    if (res.ok) {
        loadQuotes();
        loadTopQuotes();
    } else {
        alert("Failed to delete quote.");
    }
}

// Submit new quote form
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

// Tag autocomplete
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

// Filter by tag
async function filterByTag(tagName) {
    const container = document.getElementById("quotes-container");
    container.innerHTML = `<p class='text-muted text-center'>Loading quotes with tag "${tagName}"...</p>`;

    try {
        const res = await fetch(`${apiUrl}/tag/${encodeURIComponent(tagName)}`);
        const data = await res.json();

        container.innerHTML = `<h5 class="text-info">Filtered by tag: "${tagName}"</h5>`;

        if (data.length === 0) {
            container.innerHTML += "<p class='text-muted'>No quotes found with this tag.</p>";
            return;
        }

        data.forEach(q => {
            const col = document.createElement("div");
            col.className = "col-md-6 col-lg-4";

            const tags = (q.tagAssignments || [])
                .map(t => `<span class="tag" onclick="filterByTag('${t.tag.name}')">${t.tag.name}</span>`)
                .join("");

            col.innerHTML = `
                <div class="card h-100">
                    <div class="card-body">
                        <blockquote class="blockquote mb-0">
                            <p>${q.text}</p>
                            <footer class="blockquote-footer">${q.author || "Unknown"}</footer>
                            <div class="d-flex justify-content-between mt-2">
                                <span class="badge bg-secondary">Likes: ${q.likes}</span>
                                <div>
                                    <button class="btn btn-sm btn-outline-primary me-1" onclick="likeQuote(${q.id})">Like</button>
                                    <button class="btn btn-sm btn-outline-warning me-1" onclick="enableEdit(${q.id})">Edit</button>
                                    <button class="btn btn-sm btn-outline-danger" onclick="deleteQuote(${q.id})">Delete</button>
                                </div>
                            </div>
                            <div class="mt-2">${tags}</div>
                        </blockquote>
                    </div>
                </div>
            `;
            container.appendChild(col);
        });
    } catch (err) {
        console.error("Error filtering by tag:", err);
        container.innerHTML = "<p class='text-danger'>Error loading filtered quotes.</p>";
    }
}

// Initial load
loadQuotes();
loadTopQuotes();
