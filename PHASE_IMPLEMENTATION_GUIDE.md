# 🔧 Panduan Implementasi Optimasi Bandwidth - LaporanCurrentStock.razor

## 📋 Ringkasan Perubahan

Optimasi terbagi dalam 3 phase implementasi:

| Phase | Priority | Effort | Savings | Timeline |
|-------|----------|--------|---------|----------|
| **Phase 1: CSS & Conditions** | HIGH | 1-2 jam | ~40% | Hari 1 |
| **Phase 2: Virtual Scrolling** | HIGH | 2-3 jam | ~50% | Hari 1-2 |
| **Phase 3: Image Lazy Load** | MEDIUM | 2 jam | ~15% | Hari 2 |

**Total Expected Savings: 93-95% bandwidth reduction**

---

## PHASE 1: CSS Optimization + Condition Pre-Computing

### Step 1.1: Backup File Original
```powershell
cd D:\Project\BMA-PRO-NEW
git checkout -b feature/bandwidth-optimization
# Atau: Copy-Item -Path "Accounting\Pages\ModulePersediaan\Laporan\LaporanCurrentStock.razor" -Destination "LaporanCurrentStock.razor.backup"
```

### Step 1.2: Apply CSS File
**File**: `Accounting\Pages\ModulePersediaan\Laporan\LaporanCurrentStock.css` 
- Sudah dibuat
- Ganti semua inline styles dengan CSS classes

**Action**:
1. Copy/reference CSS file di project
2. Pastikan CSS ter-load (add ke layout atau global styles)

```razor
<!-- Di PrintLayout.razor atau App.razor -->
<link rel="stylesheet" href="css/LaporanCurrentStock.css" />
```

### Step 1.3: Modify IcStockCardView Model

**Opsi A: Direct Property Addition** (if model bisa dimodify)
```csharp
// Dalam IcStockCardView class
public class IcStockCardView
{
	// Existing properties...
	public string ItemCode { get; set; }
	public decimal Qty { get; set; }
	public decimal QtyBeli { get; set; }
	public decimal QtyJual { get; set; }

	// NEW PROPERTIES FOR OPTIMIZATION
	public bool HasPurchaseOrder { get; set; }     // Replace @if (item.QtyBeli != 0)
	public bool HasSalesOrder { get; set; }        // Replace @if (item.QtyJual != 0)
	public bool HasHighlightedQty { get; set; }    // For yellow row highlight
	public string QtyClass { get; set; }           // CSS class: qty-negative, qty-zero, atau ""
	public string QtyOrderClass { get; set; }      // CSS class untuk QtyOrder column
}
```

**Opsi B: Extension Properties** (if model immutable/generated)
- Gunakan file `IcStockCardViewOptimizedExtensions.cs` yang sudah dibuat
- Add extension methods untuk evaluate conditions

### Step 1.4: Update Service untuk Call EvaluateConditionals()

**Sebelum:**
```csharp
private void loadStock()
{
	TransDetail = serviceIC.GetCurrentStock();
	TransDetail = serviceOrdJual.GetCurrentOrderJual(TransDetail);
	StateHasChanged();
}
```

**Sesudah:**
```csharp
private void loadStock()
{
	TransDetail = serviceIC.GetCurrentStock();
	TransDetail = serviceOrdJual.GetCurrentOrderJual(TransDetail);

	// Pre-compute conditional properties (OPTIMIZATION)
	foreach (var item in TransDetail)
	{
		item.HasPurchaseOrder = item.QtyBeli != 0;
		item.HasSalesOrder = item.QtyJual != 0;
		item.HasHighlightedQty = item.QtyJual != 0;

		// Pre-compute CSS classes
		item.QtyClass = item.Qty < 0 ? "qty-negative" : (item.Qty == 0 ? "qty-zero" : "");
		item.QtyOrderClass = item.QtyOrder < 0 ? "qty-negative" : (item.QtyOrder == 0 ? "qty-zero" : "");
	}

	StateHasChanged();
}
```

### Step 1.5: Update Razor Template

**Replace semua `@if` conditions dengan property checks:**

| Sebelum | Sesudah |
|---------|---------|
| `@if (item.QtyBeli != 0) { <button>PO</button> }` | `@if (item.HasPurchaseOrder) { <button>PO</button> }` |
| `@if (item.QtyJual != 0) { <button>SO</button> }` | `@if (item.HasSalesOrder) { <button>SO</button> }` |
| `style="background-color:@(item.Qty < 0 ? "magenta" : (item.Qty == 0 ? "gold" : ""))"` | `class="@item.QtyClass"` |

**Reference file**: `LaporanCurrentStock-OPTIMIZED.razor` (sudah dibuat)

### Step 1.6: Extract Inline Styles

**Sebelum:**
```razor
<td style="font-family:Verdana, Geneva, sans-serif; font-weight:600; font-size:13px; 
		   border-top:1px solid #333; border-bottom:1px solid #333; 
		   border-left:1px solid #333; border-right:1px solid #333;">
```

**Sesudah:**
```razor
<td class="col-qty">
```

Referensi CSS class dari `LaporanCurrentStock.css`

### Step 1.7: Implement ShouldRender Optimization

**Tambah di @code section:**
```csharp
private bool _shouldRender = true;

protected override bool ShouldRender() => _shouldRender;

// Sebelum setiap event handler:
private void SomeEventHandler()
{
	// ... logic ...

	_shouldRender = true;      // Enable render
	StateHasChanged();
	_shouldRender = false;     // Disable untuk mencegah re-render yang tidak perlu
}
```

### Step 1.8: Test Phase 1

```powershell
# Build & test
dotnet build
dotnet run

# Test checklist:
# - [ ] Page load tanpa error
# - [ ] Styling displays correctly
# - [ ] Buttons appear sesuai condition
# - [ ] Print preview works
```

**Performance Check:**
1. Open DevTools (F12) → Network tab
2. Load laporan
3. Bandingkan total transfer size (Target: ~50% dari original)

---

## PHASE 2: Virtual Scrolling Implementation

### Prerequisites
- Blazor 8.0+ (check csproj untuk target framework)
- NuGet: `Microsoft.AspNetCore.Components.QuickGrid`

### Step 2.1: Add QuickGrid Package

```powershell
cd D:\Project\BMA-PRO-NEW
dotnet add package Microsoft.AspNetCore.Components.QuickGrid --version 8.0.0
```

### Step 2.2: Convert Table to QuickGrid

**Sebelum:**
```razor
<table>
	<thead>
		<tr>
			<th>No.</th>
			<th>Kode</th>
			...
		</tr>
	</thead>
	<tbody>
		@foreach (var item in TransDetail) { ... }
	</tbody>
</table>
```

**Sesudah:**
```razor
@using Microsoft.AspNetCore.Components.QuickGrid

<QuickGrid Items="@(TransDetail.AsQueryable())" Virtualize="true" ItemsProvider="LoadItems">
	<PropertyColumn Title="No." Property="@(x => x.No)" Sortable="false" />
	<PropertyColumn Title="Kode" Property="@(x => x.ItemCode)" />
	<PropertyColumn Title="Nama Barang" Property="@(x => x.NamaItem)" />
	<!-- ... more columns ... -->
</QuickGrid>
```

**Note**: QuickGrid membutuhkan IAsyncQueryable untuk proper async loading. Untuk now, bisa gunakan `.AsQueryable()` dengan Virtualize="true".

### Step 2.3: Add Virtual Scrolling Container

```razor
<div style="height: 60vh; overflow-y: auto;">
	<QuickGrid Items="@(TransDetail.AsQueryable())" Virtualize="true">
		<!-- Columns -->
	</QuickGrid>
</div>
```

### Step 2.4: Update CSS untuk QuickGrid

```css
.quickgrid-container {
	height: 60vh;
	overflow-y: auto;
}

.quickgrid-virtualize {
	contain: layout style paint;
}
```

### Step 2.5: Test Phase 2

```powershell
# Test dengan large dataset
# Generate test data dengan 1000+ rows

# Check:
# - [ ] Scroll performance smooth
# - [ ] Virtual rendering works (inspect DOM → should only show ~30 rows)
# - [ ] Print still works correctly
# - [ ] Buttons functionality preserved
```

**Performance Verification:**
1. Open DevTools → Performance tab
2. Scroll through 1000 rows
3. Monitor FPS (target: ≥ 60fps)
4. Check DOM size (target: ~25-30 rows rendered only)

---

## PHASE 3: Image Lazy Loading

### Step 3.1: Create Image Loading Service

```csharp
// eSoft.Persediaan.Services/IImageLoadingService.cs
public interface IImageLoadingService
{
	Task<string> GetImageBase64Async(string itemCode);
}

// Implementation
public class ImageLoadingService : IImageLoadingService
{
	private readonly HttpClient _httpClient;

	public async Task<string> GetImageBase64Async(string itemCode)
	{
		// Load image from DB/file system on demand
		var response = await _httpClient.GetAsync($"/api/images/{itemCode}");
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadAsStringAsync();
		}
		return null;
	}
}
```

### Step 3.2: Register Service

```csharp
// Program.cs atau Startup.cs
builder.Services.AddScoped<IImageLoadingService, ImageLoadingService>();
```

### Step 3.3: Update Component

```razor
@inject IImageLoadingService imageService

<!-- Sebelum: Image ada di model -->
@if (!string.IsNullOrEmpty(item.Foto))
{
	<button @onclick="@(() => OpenDisplayDialog(item))">
		Show Photo
	</button>
}

<!-- Sesudah: Load on-demand -->
<button @onclick="@(() => LoadAndShowImage(item.ItemCode))">
	Show Photo
</button>

@code {
	private async Task LoadAndShowImage(string itemCode)
	{
		imaging = await imageService.GetImageBase64Async(itemCode);
		DisplayDialogOpen = true;
		StateHasChanged();
	}
}
```

### Step 3.4: Remove Foto dari Model Transport

**Sebelum:**
```csharp
// Model include Foto (base64 string, bisa 100KB+)
public class IcStockCardView 
{
	public string Foto { get; set; }  // ❌ Tidak perlu di list
}
```

**Sesudah:**
```csharp
public class IcStockCardView 
{
	public bool HasFoto { get; set; }  // ✅ Hanya flag
}
```

---

## Testing Checklist

### Pre-Deployment Testing

#### 1. Functional Testing
- [ ] Laporan load correctly
- [ ] All data displays correctly
- [ ] Sort/filter works (if applicable)
- [ ] Print preview works
- [ ] Modal dialogs open/close correctly
- [ ] Navigation links work

#### 2. Performance Testing
```powershell
# Test dengan berbagai dataset sizes:
# - Small: 50 rows
# - Medium: 500 rows
# - Large: 5000 rows

# Measurements:
# - Initial load time (target: <2s)
# - Time to interactive (target: <1s)
# - Scroll performance (target: 60fps)
# - Memory usage (target: <50MB)
```

#### 3. Bandwidth Monitoring

**DevTools → Network tab:**
- [ ] Total transfer size (target: reduce to ~5-10% dari original)
- [ ] Number of requests (target: <10)
- [ ] SignalR message size (target: <50KB per message)

**PowerShell Bandwidth Check:**
```powershell
# Monitor real-time network usage saat load laporan
Get-NetAdapterStatistics -Name "Ethernet" | Select-Object SentBytes, ReceivedBytes
```

#### 4. Print Testing
- [ ] Print works correctly
- [ ] Page breaks are proper
- [ ] All data prints (not cut off)
- [ ] Dontprint elements hidden

#### 5. Responsive Testing
- [ ] Desktop (1920x1080)
- [ ] Tablet (768x1024)
- [ ] Mobile (375x667)

---

## Monitoring & Metrics Post-Deployment

### Key Performance Indicators (KPIs)

```csharp
// Tambah telemetry ke component:
private async Task loadStock()
{
	var startTime = DateTime.UtcNow;

	TransDetail = serviceIC.GetCurrentStock();
	TransDetail = serviceOrdJual.GetCurrentOrderJual(TransDetail);

	var loadDuration = DateTime.UtcNow - startTime;
	var memoryUsage = GC.GetTotalMemory(false);

	// Log metrics
	Console.WriteLine($"Data load time: {loadDuration.TotalMilliseconds}ms");
	Console.WriteLine($"Memory usage: {memoryUsage / 1024 / 1024}MB");
	Console.WriteLine($"Record count: {TransDetail.Count}");
}
```

### Monitoring Dashboard
Consider adding Application Insights to track:
- Page load time
- User session duration
- Error rates
- Bandwidth utilization

---

## Rollback Plan

Jika ada issue setelah deployment:

```powershell
# Revert ke original
git checkout main -- Accounting/Pages/ModulePersediaan/Laporan/LaporanCurrentStock.razor

# Atau manual restore
Copy-Item -Path "LaporanCurrentStock.razor.backup" -Destination "LaporanCurrentStock.razor"
```

---

## Documentation & Handover

1. **Code Comments**: Tambahkan "OPTIMIZATION" comment di setiap perubahan
2. **Commit Messages**: 
   ```
   feat: optimize LaporanCurrentStock bandwidth
   - Extract inline styles to CSS
   - Pre-compute conditional properties
   - Implement virtual scrolling
   - Add ShouldRender optimization

   Bandwidth reduction: ~93%
   ```
3. **Training**: Briefing tim development tentang optimization patterns
4. **Version Control**: Tag release dengan changelog

---

## FAQ & Troubleshooting

### Q: Print tidak bekerja setelah optimasi?
**A**: 
1. Check CSS @media print rules
2. Ensure print styles properly configured
3. Test: `window.print()` di DevTools console

### Q: Virtual scrolling tidak smooth?
**A**:
1. Reduce item template complexity
2. Check for expensive computations in template
3. Use `@key` directive for list items properly
4. Monitor browser DevTools Performance tab

### Q: Buttons tidak responsive?
**A**:
1. Verify event handlers properly bound (@onclick syntax)
2. Check @code section methods exist
3. Ensure StateHasChanged() called appropriately

### Q: Mobile view tidak optimal?
**A**:
1. Review media queries in CSS
2. Test with DevTools device emulation
3. Consider responsive table alternative (cards layout)

---

## Next Steps

1. **Week 1**: Implement Phase 1 + Testing
2. **Week 2**: Implement Phase 2 + Performance Tuning
3. **Week 3**: Deploy to Production + Monitor
4. **Week 4**: Gather feedback + Documentation

---

**Contact**: [Team Lead Name] untuk questions atau escalation
