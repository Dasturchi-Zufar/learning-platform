const API_URL = 'https://learning-platform-api-r962.onrender.com/api';

function getToken() {
  return localStorage.getItem('token');
}

function getUser() {
  const user = localStorage.getItem('user');
  return user ? JSON.parse(user) : null;
}

function saveAuth(token, user) {
  localStorage.setItem('token', token);
  localStorage.setItem('user', JSON.stringify(user));
}

function logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  window.location.href = '/index.html';
}

async function apiFetch(endpoint, options = {}) {
  const token = getToken();
  const headers = { 'Content-Type': 'application/json' };

  if (token) headers['Authorization'] = `Bearer ${token}`;

  const res = await fetch(`${API_URL}${endpoint}`, {
    ...options,
    headers: { ...headers, ...options.headers }
  });

  const data = await res.json();

  if (!res.ok) throw new Error(data.message || 'Xato yuz berdi');

  return data;
}

// Auth
const Auth = {
  register: (dto) => apiFetch('/auth/register', { method: 'POST', body: JSON.stringify(dto) }),
  login: (dto) => apiFetch('/auth/login', { method: 'POST', body: JSON.stringify(dto) }),
};

// Courses
const Courses = {
  getAll: (params = '') => apiFetch(`/courses${params}`),
  getById: (id) => apiFetch(`/courses/${id}`),
  create: (dto) => apiFetch('/courses', { method: 'POST', body: JSON.stringify(dto) }),
  update: (id, dto) => apiFetch(`/courses/${id}`, { method: 'PUT', body: JSON.stringify(dto) }),
  delete: (id) => apiFetch(`/courses/${id}`, { method: 'DELETE' }),
  rate: (id, rating) => apiFetch(`/courses/${id}/rate`, { method: 'POST', body: JSON.stringify({ rating }) }),
};

// Lessons
const Lessons = {
  getByCourse: (courseId) => apiFetch(`/courses/${courseId}/lessons`),
  getById: (id) => apiFetch(`/lessons/${id}`),
  create: (dto) => apiFetch('/lessons', { method: 'POST', body: JSON.stringify(dto) }),
  delete: (id) => apiFetch(`/lessons/${id}`, { method: 'DELETE' }),
};

// Progress
const Progress = {
  complete: (dto) => apiFetch('/progress/complete', { method: 'POST', body: JSON.stringify(dto) }),
  getUser: () => apiFetch('/progress/user'),
};
