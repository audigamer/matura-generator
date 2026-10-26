// --- Answer Rule Management ---
document.addEventListener("DOMContentLoaded", () => {
    const radioCustom = document.getElementById('typeCustom');
    const radioMulti = document.getElementById('typeMulti');
    const customSection = document.getElementById('customAnswerSection');
    const multiSection = document.getElementById('multiChoiceSection');
    const choicesContainer = document.getElementById('choicesContainer');
    const addChoiceBtn = document.getElementById('addChoiceBtn');

    function updateAnswerTypeVisibility() {
        if (!radioCustom || !radioMulti || !customSection || !multiSection) return;
        const isCustom = radioCustom.checked;
        customSection.classList.toggle('d-none', !isCustom);
        multiSection.classList.toggle('d-none', isCustom);
    }

    if (radioCustom && radioMulti) {
        radioCustom.addEventListener('change', updateAnswerTypeVisibility);
        radioMulti.addEventListener('change', updateAnswerTypeVisibility);
    }

    if (addChoiceBtn && choicesContainer) {
        function reindexChoices() {
            const rows = choicesContainer.querySelectorAll('.choice-row');
            rows.forEach((row, idx) => {
                const input = row.querySelector('.choice-input');
                if (input) {
                    input.name = `AnswerRule.ChoiceRules[${idx}]`;
                }
            });
        }

        addChoiceBtn.addEventListener('click', () => {
            const index = choicesContainer.querySelectorAll('.choice-row').length;
            const row = document.createElement('div');
            row.className = 'choice-row d-flex gap-2 mb-2 align-items-center';
            row.innerHTML = `
                <input type="text" class="form-control form-control-sm choice-input" name="AnswerRule.ChoiceRules[${index}]" />
                <button type="button" class="btn btn-outline-danger btn-sm remove-choice-btn">&times;</button>
            `;

            row.querySelector('.remove-choice-btn').addEventListener('click', () => {
                row.remove();
                reindexChoices();
            });

            choicesContainer.appendChild(row);
            row.querySelector('.choice-input').focus();
        });

        choicesContainer.querySelectorAll('.remove-choice-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.target.closest('.choice-row').remove();
                reindexChoices();
            });
        });
    }
});
