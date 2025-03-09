// 檢查是否已登入
function checkAuth() {
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    
    if (!token) {
        // 如果不在登入頁且未登入，重定向到登入頁
        if (!window.location.pathname.includes('login.html')) {
            window.location.href = '/login.html';
        }
        return false;
    }
    
    // 更新導航欄
    updateNavbar(user);
    return true;
}

// 更新導航欄
function updateNavbar(user) {
    $('#userInfo').text(`${user.name || '用戶'} (${user.role})`);
    
    // 根據角色顯示不同的選項
    if (user.role === 'Admin') {
        // 可以添加管理員專用的導航項目
        $('#navbarNav .navbar-nav:first').append(`
            <li class="nav-item">
                <a class="nav-link" href="/admin.html">管理後台</a>
            </li>
        `);
    }
}

// 登出
function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login.html';
}

// 設置AJAX默認的Authorization頭
$.ajaxSetup({
    beforeSend: function(xhr) {
        const token = localStorage.getItem('token');
        if (token) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        }
    }
});

// 處理API錯誤
function handleApiError(error) {
    console.error('API Error:', error);
    
    if (error.status === 401) {
        alert('您的登入已過期，請重新登入');
        logout();
    } else {
        let errorMessage = '操作失敗';
        try {
            const response = JSON.parse(error.responseText);
            errorMessage = response.message || errorMessage;
        } catch (e) {
            errorMessage = error.statusText || errorMessage;
        }
        return errorMessage;
    }
}

// 格式化日期時間
function formatDateTime(dateString) {
    if (!dateString) return '';
    
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    
    return `${year}-${month}-${day} ${hours}:${minutes}`;
}

// 文檔就緒時執行
$(document).ready(function() {
    // 檢查認證
    checkAuth();
    
    // 設置登出事件
    $('#logoutLink').on('click', function(e) {
        e.preventDefault();
        logout();
    });
}); 