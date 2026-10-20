document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById('statementInput');
    const container = document.getElementById('domainsContainer');
    const emptyState = document.getElementById('emptyState');
    const template = document.getElementById('domainCardTemplate');

    if (!input || !container || !template) return;

    function sync() {
        const matches = [...new Set((input.value.match(/@[a-zA-Z]/g) || []).map(m => m.slice(1)))];

        if (emptyState) {
            emptyState.style.display = matches.length ? 'none' : 'block';
        }

        // Remove cards for constants no longer in the statement
        container.querySelectorAll('.domain-card').forEach(card => {
            if (!matches.includes(card.dataset.constant)) {
                card.remove();
            }
        });

        // Add cards for newly typed constants
        matches.forEach(c => {
            let card = container.querySelector(`.domain-card[data-constant="${c}"]`);
            if (!card) {
                const clone = template.content.cloneNode(true);
                card = clone.querySelector('.domain-card');
                card.dataset.constant = c;
                card.querySelector('.domain-badge').textContent = `@${c}`;

                card.querySelector('.type-select').addEventListener('change', () => {
                    updateTypeVisibility(card);
                });

                container.appendChild(card);
            }
        });

        // Re-index all domain inputs so ASP.NET model binder maps to Domains[0], Domains[1], etc.
        const allCards = container.querySelectorAll('.domain-card');
        allCards.forEach((card, idx) => {
            card.querySelector('.domain-name').name = `Domains[${idx}].ConstantName`;
            card.querySelector('.domain-name').value = card.dataset.constant;
            card.querySelector('.type-select').name = `Domains[${idx}].Type`;
            card.querySelector('.min-val').name = `Domains[${idx}].MinValue`;
            card.querySelector('.max-val').name = `Domains[${idx}].MaxValue`;
            card.querySelector('.min-num').name = `Domains[${idx}].MinNumerator`;
            card.querySelector('.max-num').name = `Domains[${idx}].MaxNumerator`;
            card.querySelector('.min-den').name = `Domains[${idx}].MinDenominator`;
            card.querySelector('.max-den').name = `Domains[${idx}].MaxDenominator`;
        });
    }

    function updateTypeVisibility(card) {
        const isFraction = card.querySelector('.type-select').value === 'fraction';
        card.querySelector('.int-fields').classList.toggle('d-none', isFraction);
        card.querySelector('.fraction-fields').classList.toggle('d-none', !isFraction);
    }

    // Attach type-toggle listeners to any pre-rendered cards (e.g. from Edit view)
    container.querySelectorAll('.domain-card').forEach(card => {
        card.querySelector('.type-select').addEventListener('change', () => updateTypeVisibility(card));
    });

    input.addEventListener('input', sync);
    sync(); // Run once on page load

    // --- Rules Management ---
    const addRuleBtn = document.getElementById('addRuleBtn');
    const rulesContainer = document.getElementById('rulesContainer');
    const emptyRulesState = document.getElementById('emptyRulesState');

    if (addRuleBtn && rulesContainer) {
        function reindexRules() {
            const rows = rulesContainer.querySelectorAll('.rule-row');
            if (emptyRulesState) {
                emptyRulesState.style.display = rows.length ? 'none' : 'block';
            }
            rows.forEach((row, idx) => {
                const input = row.querySelector('.rule-input');
                if (input) {
                    input.name = `Rules[${idx}]`;
                }
            });
        }

        addRuleBtn.addEventListener('click', () => {
            const row = document.createElement('div');
            row.className = 'rule-row d-flex gap-2 mb-2 align-items-center';
            row.innerHTML = `
                <input type="text" class="form-control form-control-sm rule-input" placeholder="e.g. gcd(A, B) == 1" />
                <button type="button" class="btn btn-outline-danger btn-sm remove-rule-btn">&times;</button>
            `;

            row.querySelector('.remove-rule-btn').addEventListener('click', () => {
                row.remove();
                reindexRules();
            });

            rulesContainer.appendChild(row);
            reindexRules();
            row.querySelector('.rule-input').focus();
        });

        // Attach delete listener to any pre-existing rule rows (e.g. from Edit view)
        rulesContainer.querySelectorAll('.remove-rule-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.target.closest('.rule-row').remove();
                reindexRules();
            });
        });

        reindexRules();
    }
});
