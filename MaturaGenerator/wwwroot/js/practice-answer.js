document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('answerForm');
    if (!form) return;

    const slider = document.getElementById('answerSlider');
    const sliderValue = document.getElementById('sliderValue');
    const submitBtn = document.getElementById('submitBtn');
    const resultContainer = document.getElementById('resultContainer');

    if (slider && sliderValue) {
        slider.addEventListener('input', function () {
            sliderValue.textContent = this.value;
        });
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        
        const checkUrl = form.dataset.checkUrl || form.action;
        const formData = new FormData(form);
        const params = new URLSearchParams(formData);

        if (submitBtn) submitBtn.disabled = true;
        if (slider) slider.disabled = true;
        
        try {
            const response = await fetch(`${checkUrl}?${params.toString()}`);
            if (!response.ok) {
                throw new Error(`Request failed with status ${response.status}`);
            }
            const html = await response.text();
            if (resultContainer) {
                resultContainer.innerHTML = html;
            }
        } catch (err) {
            if (resultContainer) {
                resultContainer.innerHTML = '<div class="alert alert-danger mt-3">Error submitting answer. Please try again.</div>';
            }
            if (submitBtn) submitBtn.disabled = false;
            if (slider) slider.disabled = false;
        }
    });
});
