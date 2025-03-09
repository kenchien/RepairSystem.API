$(document).ready(function() {
    // 移除之前的Token（如果已登入）
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    
    // 處理登入表單提交
    $('#loginForm').on('submit', function(e) {
        e.preventDefault();
        
        const username = $('#username').val();
        const password = $('#password').val();
        
        // 隱藏錯誤訊息
        $('#loginError').addClass('d-none');
        
        // 發送登入請求
        $.ajax({
            url: '/api/auth/login',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ username, password }),
            success: function(response) {
                // 將 token 保存到本地存儲
                localStorage.setItem('token', response.token);
                localStorage.setItem('user', JSON.stringify(response.user));
                
                // 重定向到首頁
                window.location.href = '/repair.html';
            },
            error: function(xhr) {
                // 顯示錯誤訊息
                let errorMessage = '登入失敗';
                
                try {
                    const response = JSON.parse(xhr.responseText);
                    errorMessage = response.message || errorMessage;
                } catch (e) {
                    errorMessage = xhr.statusText || errorMessage;
                }
                
                $('#loginError').text(errorMessage).removeClass('d-none');
            }
        });
    });
}); 