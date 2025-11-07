// API Base URL
const API_BASE = '/api/lawyers';

// DOM Elements
const searchForm = document.getElementById('searchForm');
const resetBtn = document.getElementById('resetBtn');
const stateSelect = document.getElementById('state');
const citySelect = document.getElementById('city');
const minYearsInput = document.getElementById('minYears');
const maxYearsInput = document.getElementById('maxYears');
const validationError = document.getElementById('validationError');
const resultsSection = document.getElementById('resultsSection');
const resultsBody = document.getElementById('resultsBody');
const resultCount = document.getElementById('resultCount');
const noResults = document.getElementById('noResults');
const loading = document.getElementById('loading');
const practiceList = document.getElementById('practiceList');

// Initialize the app
document.addEventListener('DOMContentLoaded', () => {
    loadStates();
    loadPracticeAreas();
    setupEventListeners();
});

// Setup Event Listeners
function setupEventListeners() {
    searchForm.addEventListener('submit', handleSearch);
    resetBtn.addEventListener('click', resetForm);
    stateSelect.addEventListener('change', handleStateChange);

    // Validate years inputs
    minYearsInput.addEventListener('input', validateYearsInput);
    maxYearsInput.addEventListener('input', validateYearsInput);
}

// Load States from API
async function loadStates() {
    try {
        const response = await fetch(`${API_BASE}/states`);
        const states = await response.json();

        stateSelect.innerHTML = '<option value="">-- All States --</option>';
        states.forEach(state => {
            const option = document.createElement('option');
            option.value = state;
            option.textContent = state;
            stateSelect.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading states:', error);
        showError('Failed to load states. Please refresh the page.');
    }
}

// Load Practice Areas from API
async function loadPracticeAreas() {
    try {
        const response = await fetch(`${API_BASE}/practices`);
        const practices = await response.json();

        practiceList.innerHTML = '';
        practices.forEach(practice => {
            const option = document.createElement('option');
            option.value = practice;
            practiceList.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading practice areas:', error);
    }
}

// Handle State Change
async function handleStateChange(e) {
    const selectedState = e.target.value;

    if (!selectedState) {
        citySelect.innerHTML = '<option value="">-- Select State First --</option>';
        citySelect.disabled = true;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/cities?state=${encodeURIComponent(selectedState)}`);
        const cities = await response.json();

        citySelect.innerHTML = '<option value="">-- All Cities --</option>';
        cities.forEach(city => {
            const option = document.createElement('option');
            option.value = city;
            option.textContent = city;
            citySelect.appendChild(option);
        });

        citySelect.disabled = false;
    } catch (error) {
        console.error('Error loading cities:', error);
        showError('Failed to load cities for the selected state.');
    }
}

// Validate Years Input
function validateYearsInput() {
    const minYears = parseInt(minYearsInput.value) || -1;
    const maxYears = parseInt(maxYearsInput.value) || -1;

    // Clear previous error
    hideError();

    // Validate minimum years >= 0
    if (minYears < -1 || (minYearsInput.value && minYears < 0)) {
        showError('Minimum years of experience cannot be less than 0.');
        return false;
    }

    // Validate maximum years >= 0
    if (maxYears < -1 || (maxYearsInput.value && maxYears < 0)) {
        showError('Maximum years of experience cannot be less than 0.');
        return false;
    }

    // Validate max >= min
    if (minYears >= 0 && maxYears >= 0 && maxYears < minYears) {
        showError('Maximum years must be greater than or equal to minimum years.');
        return false;
    }

    return true;
}

// Handle Search Form Submit
async function handleSearch(e) {
    e.preventDefault();

    // Validate inputs
    if (!validateYearsInput()) {
        return;
    }

    hideError();

    // Build query parameters
    const formData = new FormData(searchForm);
    const params = new URLSearchParams();

    for (const [key, value] of formData.entries()) {
        if (value.trim()) {
            params.append(key, value.trim());
        }
    }

    // Show loading state
    showLoading();

    try {
        const response = await fetch(`${API_BASE}/search?${params.toString()}`);
        const lawyers = await response.json();

        hideLoading();
        displayResults(lawyers);
    } catch (error) {
        console.error('Error searching lawyers:', error);
        hideLoading();
        showError('Failed to search lawyers. Please try again.');
    }
}

// Display Results
function displayResults(lawyers) {
    resultsBody.innerHTML = '';

    if (lawyers.length === 0) {
        resultsSection.style.display = 'block';
        noResults.style.display = 'block';
        document.querySelector('.table-container').style.display = 'none';
        resultCount.textContent = '';
        return;
    }

    // Show results section
    resultsSection.style.display = 'block';
    noResults.style.display = 'none';
    document.querySelector('.table-container').style.display = 'block';
    resultCount.textContent = `${lawyers.length} lawyer${lawyers.length !== 1 ? 's' : ''} found`;

    // Create table rows
    lawyers.forEach(lawyer => {
        const row = createLawyerRow(lawyer);
        resultsBody.appendChild(row);
    });

    // Scroll to results
    resultsSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

// Create Lawyer Table Row
function createLawyerRow(lawyer) {
    const row = document.createElement('tr');

    row.innerHTML = `
        <td data-label="Picture">
            <img src="${lawyer.profilePictureUrl || 'https://via.placeholder.com/60'}"
                 alt="${lawyer.name}"
                 class="lawyer-photo"
                 onerror="this.src='https://via.placeholder.com/60?text=No+Image'">
        </td>
        <td data-label="Name" class="lawyer-name">${escapeHtml(lawyer.name)}</td>
        <td data-label="Area of Practice" class="practice-area">${escapeHtml(lawyer.areaOfPractice)}</td>
        <td data-label="Description" class="description">${escapeHtml(lawyer.description)}</td>
        <td data-label="Years of Experience">
            <span class="years-badge">${lawyer.yearsOfExperience} years</span>
        </td>
        <td data-label="State" class="location">${escapeHtml(lawyer.state)}</td>
        <td data-label="City" class="location">${escapeHtml(lawyer.city)}</td>
        <td data-label="Website">
            ${lawyer.website
                ? `<a href="${escapeHtml(lawyer.website)}" target="_blank" rel="noopener noreferrer" class="website-link">Visit Website</a>`
                : 'N/A'}
        </td>
        <td data-label="AmLaw Ranking">
            <span class="ranking ${lawyer.amlawRanking === 'NR' ? 'nr' : ''}">${escapeHtml(lawyer.amlawRanking)}</span>
        </td>
    `;

    return row;
}

// Reset Form
function resetForm() {
    searchForm.reset();
    citySelect.innerHTML = '<option value="">-- Select State First --</option>';
    citySelect.disabled = true;
    hideError();
    resultsSection.style.display = 'none';
}

// Show Error Message
function showError(message) {
    validationError.textContent = message;
    validationError.classList.add('show');
}

// Hide Error Message
function hideError() {
    validationError.textContent = '';
    validationError.classList.remove('show');
}

// Show Loading State
function showLoading() {
    loading.style.display = 'block';
    resultsSection.style.display = 'none';
}

// Hide Loading State
function hideLoading() {
    loading.style.display = 'none';
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}
