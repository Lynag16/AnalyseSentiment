document.getElementById('sentiment-form').addEventListener('submit', async function(event) {
    event.preventDefault();

    const text = document.getElementById('sentiment-input').value;
    const response = await fetch('http://localhost:5139/api/sentiment/predict', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(text)
    });

    if (response.ok) {
        const data = await response.json();
        document.getElementById('result').innerText = `Sentiment: ${data.sentiment} with probability: ${data.probability}`;
    } else {
        document.getElementById('result').innerText = 'Error analyzing sentiment.';
    }
});
