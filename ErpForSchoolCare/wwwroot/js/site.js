
document.getElementById("toggleSidebar").addEventListener("click", function (){
    document.getElementById("sidebar").classList.toggle("collapsed");
});

let timerInterval;

async function startSessionTimer() {

	const response = await fetch('/Account/SessionInfo');

	if (!response.ok) {
		document.getElementById("countdown").innerText = "Expired";
		return;
	}

	const data = await response.json();
	let expiryTime = new Date(data.expiresAt);

	if (timerInterval)
		clearInterval(timerInterval);

	timerInterval = setInterval(() => {
		const now = new Date();
		const diff = expiryTime - now;

		if (diff <= 0) {
			clearInterval(timerInterval);
			document.getElementById("countdown").innerText = "Expired";
			return;
		}

		const minutes = Math.floor(diff / 60000);
		const seconds = Math.floor((diff % 60000) / 1000);

		document.getElementById("countdown").innerText =
			`${minutes}:${seconds.toString().padStart(2, '0')}`;

		if (minutes < 2) {
			document.getElementById("session-timer").style.color = "red";
		}
	}, 1000);
}

startSessionTimer();
