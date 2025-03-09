$(document).ready(function() {
    initEquipmentTable();
});

function initEquipmentTable() {
    $('#equipmentTable').DataTable({
        ajax: {
            url: '/api/equipment',
            dataSrc: ''
        },
        columns: [
            { data: 'serialNumber' },
            { data: 'name' },
            { data: 'type' },
            { data: 'location' },
            { data: 'department' },
            { 
                data: 'status',
                render: function(data) {
                    const statusClasses = {
                        '正常': 'success',
                        '維修中': 'warning',
                        '報廢': 'danger'
                    };
                    return `<span class="badge bg-${statusClasses[data] || 'secondary'}">${data}</span>`;
                }
            },
            { 
                data: 'lastMaintenanceDate',
                render: function(data) {
                    return data ? new Date(data).toLocaleDateString() : '無';
                }
            },
            {
                data: 'id',
                render: function(data) {
                    return `
                        <button class="btn btn-sm btn-info" onclick="viewEquipment(${data})">查看</button>
                        <button class="btn btn-sm btn-primary" onclick="addMaintenance(${data})">維護記錄</button>
                    `;
                }
            }
        ],
        order: [[0, 'asc']],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.10.24/i18n/Chinese-traditional.json'
        }
    });
}

function saveEquipment() {
    const formData = new FormData($('#equipmentForm')[0]);
    const equipment = Object.fromEntries(formData.entries());

    $.ajax({
        url: '/api/equipment',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(equipment),
        success: function(response) {
            $('#addEquipmentModal').modal('hide');
            $('#equipmentTable').DataTable().ajax.reload();
            alert('設備添加成功');
        },
        error: function(xhr) {
            alert('設備添加失敗：' + xhr.responseText);
        }
    });
}

function addMaintenance(equipmentId) {
    // 實現添加維護記錄的功能
}

function viewEquipment(equipmentId) {
    // 實現查看設備詳情的功能
} 