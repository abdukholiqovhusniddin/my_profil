const likeButton = document.getElementById("like-button");
const likeButton2 = document.getElementById("like-button2");
const likeCountEl = document.getElementById("like-count");

if (likeButton && likeButton2 && likeCountEl) {
    likeButton.addEventListener("click", async () => {
        likeButton.style.display = "none";
        likeButton2.style.display = "inline-block";
        likeButton2.classList.add("clicked");

        try {
            const res = await fetch("/Home/AddLike", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({})
            });

            if (res.ok) {
                const data = await res.json();
                likeCountEl.textContent = data.likecount;
            } else {
                alert("Failed to update like count.");
            }
        } catch (err) {
            console.error("Error adding like:", err);
            alert("Connection error while updating like.");
        }
    });

    likeButton2.addEventListener("click", async () => {
        likeButton.style.display = "inline-block";
        likeButton2.style.display = "none";

        try {
            const res = await fetch("/Home/RemoveLike", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({})
            });

            if (res.ok) {
                const data = await res.json();
                likeCountEl.textContent = data.likecount;
            } else {
                alert("Failed to update like count.");
            }
        } catch (err) {
            console.error("Error removing like:", err);
            alert("Connection error while updating like.");
        }
    });
}

const messageBtn = document.getElementById("message");

if (messageBtn) {
    messageBtn.addEventListener("click", async () => {
        const userInput = prompt("Enter your message:");
        if (!userInput || !userInput.trim()) {
            alert("Please enter a valid message.");
            return;
        }

        try {
            const res = await fetch("/Home/SendToTelegram", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ text: userInput })
            });

            if (res.ok) {
                alert("Your message has been sent!");
            } else {
                alert("Failed to send the message.");
            }
        } catch (err) {
            console.error("Error sending message:", err);
            alert("Connection error while sending message.");
        }
    });
}

const shareBtn = document.getElementById("sharebutton");
const shareCountEl = document.getElementById("countshare");

if (shareBtn && shareCountEl) {
    shareBtn.addEventListener("click", async () => {
        if (navigator.share) {
            try {
                await navigator.share({
                    title: "Abdukholiqov Husniddin - Personal Portfolio",
                    text: "Learn more about me.",
                    url: window.location.href
                });

                const res = await fetch("/Home/ShareCount", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({})
                });

                if (res.ok) {
                    const data = await res.json();
                    shareCountEl.textContent = data.countshare;
                }
            } catch (err) {
                console.log("Share failed:", err);
                alert("Sharing was canceled or failed.");
            }
        } else {
            alert("This browser does not support sharing.");
        }
    });
}