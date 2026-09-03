window.getAuditIpAddress = async function () {
    const response = await fetch('/audit/client-ip', { credentials: 'include' });
    return response.ok ? await response.text() : '';
};