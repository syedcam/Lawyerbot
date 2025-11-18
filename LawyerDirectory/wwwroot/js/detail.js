// API Base URL
const API_BASE = '/api/lawyers';

// DOM Elements
const loading = document.getElementById('loading');
const lawyerDetail = document.getElementById('lawyerDetail');
const errorMessage = document.getElementById('errorMessage');

// Get lawyer ID from URL parameters
function getLawyerId() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id');
}

// Load lawyer details
async function loadLawyerDetails() {
    const lawyerId = getLawyerId();

    if (!lawyerId) {
        showError();
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/${lawyerId}`);
        const lawyer = await response.json();

        if (lawyer.error) {
            showError();
            return;
        }

        displayLawyer(lawyer);
    } catch (error) {
        console.error('Error loading lawyer details:', error);
        showError();
    }
}

// Display lawyer information
function displayLawyer(lawyer) {
    // Hide loading, show details
    loading.style.display = 'none';
    lawyerDetail.style.display = 'block';

    // Set photo - use black square if no picture available
    const photoUrl = lawyer.profilePictureUrl || 'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22200%22 height=%22200%22%3E%3Crect width=%22200%22 height=%22200%22 fill=%22%23000000%22/%3E%3C/svg%3E';
    document.getElementById('lawyerPhoto').src = photoUrl;
    document.getElementById('lawyerPhoto').alt = lawyer.name;
    document.getElementById('lawyerPhoto').onerror = function() {
        this.src = 'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22200%22 height=%22200%22%3E%3Crect width=%22200%22 height=%22200%22 fill=%22%23000000%22/%3E%3C/svg%3E';
    };

    // Set text fields
    document.getElementById('firstName').textContent = lawyer.firstName || 'N/A';
    document.getElementById('lastName').textContent = lawyer.lastName || 'N/A';
    document.getElementById('company').textContent = lawyer.company || 'N/A';
    document.getElementById('email').innerHTML = lawyer.email
        ? `<a href="mailto:${lawyer.email}">${lawyer.email}</a>`
        : 'N/A';
    document.getElementById('phone').innerHTML = lawyer.phone
        ? `<a href="tel:${lawyer.phone}">${lawyer.phone}</a>`
        : 'N/A';
    document.getElementById('areaOfPractice').textContent = lawyer.areaOfPractice || 'N/A';
    document.getElementById('yearsOfExperience').textContent = lawyer.yearsOfExperience
        ? `${lawyer.yearsOfExperience} years`
        : 'N/A';
    document.getElementById('city').textContent = lawyer.city || 'N/A';
    document.getElementById('state').textContent = lawyer.state || 'N/A';
    document.getElementById('amlawRanking').textContent = lawyer.amlawRanking || 'NR';

    // Set LinkedIn URL
    if (lawyer.linkedinUrl) {
        document.getElementById('linkedinUrl').innerHTML =
            `<a href="${lawyer.linkedinUrl}" target="_blank" rel="noopener noreferrer">View LinkedIn Profile →</a>`;
    } else {
        document.getElementById('linkedinUrl').textContent = 'N/A';
    }

    // Set Company Profile URL
    if (lawyer.companyProfileUrl) {
        const url = lawyer.companyProfileUrl.startsWith('http')
            ? lawyer.companyProfileUrl
            : `https://${lawyer.companyProfileUrl}`;
        document.getElementById('companyProfileUrl').innerHTML =
            `<a href="${url}" target="_blank" rel="noopener noreferrer">View Firm Profile →</a>`;
    } else {
        document.getElementById('companyProfileUrl').textContent = 'N/A';
    }

    // Update page title
    document.title = `${lawyer.name} - Lawyer Directory`;
}

// Show error message
function showError() {
    loading.style.display = 'none';
    errorMessage.style.display = 'block';
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', loadLawyerDetails);
