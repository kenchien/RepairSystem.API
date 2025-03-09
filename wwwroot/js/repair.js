// 文件上傳處理
function handleFileUpload(ticketId, files) {
    const formData = new FormData();
    for (let file of files) {
        formData.append('file', file);
    }

    $.ajax({
        url: `/api/repair/${ticketId}/attachments`,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function(response) {
            updateFileList(ticketId);
        },
        error: function(xhr) {
            alert('文件上傳失敗');
        }
    });
}

// 更新統計數據
function updateStatistics() {
    $.get('/api/report/statistics', function(data) {
        $('#pendingCount').text(data.pending);
        $('#processingCount').text(data.processing);
        $('#completedCount').text(data.completed);
        $('#totalCount').text(data.total);
    });
}

// 導出報表
function exportReport() {
    window.location.href = '/api/report/export';
}

// 定期更新統計數據
setInterval(updateStatistics, 60000); // 每分鐘更新一次

$(document).ready(function() {
    // 確認用戶已登入
    if (!checkAuth()) return;
    
    // 載入報修單列表
    loadTickets();
    
    // 綁定刷新按鈕
    $('#refreshBtn').on('click', loadTickets);
    
    // 綁定報修表單提交
    $('#repairForm').on('submit', function(e) {
        e.preventDefault();
        submitRepair();
    });
    
    // 綁定保存報修單詳情
    $('#saveTicketBtn').on('click', saveTicket);
});

// 載入報修單列表
function loadTickets() {
    $('#ticketsLoading').show();
    $('#noTickets').addClass('d-none');
    $('#ticketTable tbody').empty();
    
    $.ajax({
        url: '/api/repair',
        type: 'GET',
        success: function(tickets) {
            $('#ticketsLoading').hide();
            
            if (tickets.length === 0) {
                $('#noTickets').removeClass('d-none');
                return;
            }
            
            const user = JSON.parse(localStorage.getItem('user') || '{}');
            const isUserOrTech = user.role === 'User' || user.role === 'Technician';
            
            tickets.forEach(function(ticket) {
                // 如果是普通用戶，只顯示自己的報修單
                if (isUserOrTech && user.role === 'User' && ticket.userId !== user.userId) {
                    return;
                }
                
                const row = `
                    <tr>
                        <td>${ticket.ticketId}</td>
                        <td>${ticket.deviceType}</td>
                        <td>${truncateText(ticket.problem, 50)}</td>
                        <td><span class="status-badge ${getStatusClass(ticket.status)}">${ticket.status}</span></td>
                        <td>${formatDateTime(ticket.createTime)}</td>
                        <td>
                            <button class="btn btn-sm btn-primary" onclick="viewTicket(${ticket.ticketId})">查看</button>
                        </td>
                    </tr>
                `;
                $('#ticketTable tbody').append(row);
            });
        },
        error: function(xhr) {
            $('#ticketsLoading').hide();
            alert(handleApiError(xhr));
        }
    });
}

// 提交報修單
function submitRepair() {
    const repair = {
        deviceType: $('[name="deviceType"]').val(),
        deviceNumber: $('[name="deviceNumber"]').val(),
        problem: $('[name="problem"]').val(),
        priority: $('[name="priority"]').val()
    };
    
    $.ajax({
        url: '/api/repair',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(repair),
        success: function(response) {
            alert('報修單提交成功！');
            $('#repairForm')[0].reset();
            loadTickets();
        },
        error: function(xhr) {
            alert(handleApiError(xhr));
        }
    });
}

// 查看報修單詳情
function viewTicket(id) {
    $.ajax({
        url: `/api/repair/${id}`,
        type: 'GET',
        success: function(ticket) {
            $('#ticketId').val(ticket.ticketId);
            $('#modalDeviceType').val(ticket.deviceType);
            $('#modalDeviceNumber').val(ticket.deviceNumber);
            $('#modalProblem').val(ticket.problem);
            $('#modalUser').val(ticket.user ? ticket.user.name : '未知');
            $('#modalCreateTime').val(formatDateTime(ticket.createTime));
            $('#modalPriority').val(ticket.priority);
            $('#modalStatus').val(ticket.status);
            $('#modalSolution').val(ticket.solution || '');
            
            // 根據角色設置可編輯性
            const user = JSON.parse(localStorage.getItem('user') || '{}');
            const canEdit = user.role === 'Admin' || user.role === 'Technician';
            
            $('#modalPriority').prop('disabled', !canEdit);
            $('#modalStatus').prop('disabled', !canEdit);
            $('#modalSolution').prop('disabled', !canEdit);
            $('#saveTicketBtn').toggle(canEdit);
            
            // 顯示模態框
            const modal = new bootstrap.Modal(document.getElementById('ticketModal'));
            modal.show();
        },
        error: function(xhr) {
            alert(handleApiError(xhr));
        }
    });
}

// 保存報修單更新
function saveTicket() {
    const ticket = {
        ticketId: $('#ticketId').val(),
        deviceType: $('#modalDeviceType').val(),
        deviceNumber: $('#modalDeviceNumber').val(),
        problem: $('#modalProblem').val(),
        priority: $('#modalPriority').val(),
        status: $('#modalStatus').val(),
        solution: $('#modalSolution').val(),
        handledBy: JSON.parse(localStorage.getItem('user') || '{}').userId
    };
    
    $.ajax({
        url: `/api/repair/${ticket.ticketId}`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(ticket),
        success: function(response) {
            alert('報修單更新成功！');
            $('#ticketModal').modal('hide');
            loadTickets();
        },
        error: function(xhr) {
            alert(handleApiError(xhr));
        }
    });
}

// 獲取狀態對應的樣式類
function getStatusClass(status) {
    switch (status) {
        case '待處理':
            return 'waiting';
        case '處理中':
            return 'processing';
        case '已完成':
            return 'completed';
        case '已關閉':
            return 'closed';
        default:
            return '';
    }
}

// 截斷文本，超過長度添加省略號
function truncateText(text, maxLength) {
    if (!text) return '';
    return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
} 