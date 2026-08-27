# 📋 Laporan Optimasi Bandwidth - LaporanCurrentStock.razor

## 🎯 Prioritas Optimasi (Impact vs Effort)

### **PRIORITY 1 - High Impact, Low Effort (IMPLEMENTASI SEGERA)**

#### ✅ 1.1 Ekstrak Inline Styles ke CSS (Hemat ~30-50%)
**Masalah:** Setiap `<td>` dan `<tr>` memiliki inline style yang identik

**Dampak:** Jika ada 1000 rows:
- Saat ini: 8 columns × 1000 rows × ~200 bytes/style = ~1.6 MB hanya untuk styles
- Setelah fix: ~5 KB CSS file + DOM referensi = ~99% pengurangan

**Solusi:**
```css
/* Tambah di PrintLayout.razor atau global CSS */
.td-header { 
	font-family: Verdana, Geneva, sans-serif;
	font-weight: 600; 
	font-size: 13px; 
	border: 1px solid #333;
}

.td-content {
	font-family: Verdana, Geneva, sans-serif;
	font-weight: 300;
	font-size: 11px;
}

.qty-negative { background-color: magenta; }
.qty-zero { background-color: gold; }
.qty-highlight { background-color: yellow; }

.col-number { width: 5%; text-align: left; }
.col-code { width: 10%; text-align: center; }
.col-name { width: 40%; text-align: left; }
.col-unit { width: 5%; text-align: right; }
.col-qty { width: 10%; text-align: right; }
.col-po { width: 10%; text-align: right; }
.col-so { width: 10%; text-align: right; }
.col-total { width: 10%; text-align: right; border-right: 1px solid #333; }
```

---

#### ✅ 1.2 Implementasi Virtual Scrolling (Hemat ~60-80%)
**Masalah:** Renderisasi 1000+ rows sekaligus = SignalR message besar

**Solusi:** Gunakan QuickGrid dengan Virtual Scrolling (Blazor 8.0+)
- Hanya render ~20-30 rows visible di viewport
- Rest dimuat on-demand
- Reduction: 1000 rows → ~25 rows rendered = 96% DOM yang lebih kecil

---

#### ✅ 1.3 Pre-compute Conditional Buttons (Hemat ~15-25%)
**Masalah:** Setiap render, logic `item.QtyBeli != 0` dievaluasi di template

**Solusi:**
```csharp
// Di service/model, tambah property computed
public class IcStockCardView {
	public bool HasPurchaseOrder => QtyBeli != 0;
	public bool HasSalesOrder => QtyJual != 0;
	public string QtyClass => Qty < 0 ? "qty-negative" : (Qty == 0 ? "qty-zero" : "");
}
```

Di template:
```razor
@if (item.HasPurchaseOrder)
{
	<button>PO</button>
}
```

---

### **PRIORITY 2 - Medium Impact, Medium Effort (IMPLEMENTASI BERIKUTNYA)**

#### ✅ 2.1 Image Lazy Loading & Base64 Optimization
**Masalah:** Foto base64 besar ditransmit di setiap update SignalR

**Solusi:**
- Jangan embed foto di list → load on-demand di modal
- Resize image sebelum base64
- Gunakan WebP format (60-80% lebih kecil dari PNG/JPG)

```razor
<!-- Sebelum -->
<button @onclick="() => OpenDisplayDialog(item)">
	<!-- Full image base64 data dibawa di seluruh list -->
</button>

<!-- Sesudah -->
<button @onclick="() => LoadImageOnDemand(item.ItemCode)">
	<!-- Hanya bawa ItemCode, load image dari server saat modal buka -->
</button>
```

---

#### ✅ 2.2 Batch Updates dengan ShouldRender
**Masalah:** Setiap perubahan kecil trigger re-render full page

**Solusi:**
```csharp
private bool _shouldRender = true;

protected override bool ShouldRender() => _shouldRender;

private void ToggleRowsWithSameDivision(string kodeDivisi)
{
	var itemsToToggle = TransDetail.Where(x => x.KodeDivisi == kodeDivisi);
	foreach (var item in itemsToToggle)
	{
		item.IsSelectedForPrinting = !item.IsSelectedForPrinting;
	}

	_shouldRender = true;  // Hanya mark perlu render
	StateHasChanged();
	_shouldRender = false; // Reset
}
```

---

#### ✅ 2.3 Pagination Fallback (jika virtualization tidak applicable)
**Limit:** 50-100 rows per page (user tetap bisa scroll dalam page)
- Hanya load data visible
- Add "Load More" button
- Reduce initial payload: ~1000 rows → 50 rows = 95% pengurangan initial

---

### **PRIORITY 3 - Low Impact, High Effort (OPTIONAL/FUTURE)**

#### 3.1 Implementasi Filtering/Search di Server-side
- Hanya kirim filtered results
- Reduce data transfer saat user search

#### 3.2 Compress SVG Icons
- Gunakan icon library daripada inline SVG
- Atau minify SVG

#### 3.3 Render-to-static Approach
- Jika laporan ini mostly read-only → export ke PDF/Excel
- Blazor interactive tidak perlu untuk report yang jarang update

---

## 📊 Estimasi Impact

| Optimasi | Bandwidth Before | Bandwidth After | Saving | Prioritas |
|----------|------------------|-----------------|--------|-----------|
| CSS Instead of Inline | 1.6 MB | 0.05 MB | 96.9% | P1 |
| Virtual Scrolling | 500 KB | 20 KB | 96% | P1 |
| Pre-compute Conditions | 50 KB | 30 KB | 40% | P1 |
| Image Lazy Load | 200 KB | 20 KB | 90% | P2 |
| Pagination | 500 KB | 50 KB | 90% | P2 |
| **TOTAL (Best Case)** | ~2.75 MB | ~0.18 MB | **93.5%** | - |

---

## 🔧 Implementasi Checklist

### Phase 1 - Quick Wins (1-2 hari):
- [ ] Extract styles ke CSS file
- [ ] Pre-compute conditional button visibility
- [ ] Implement ShouldRender optimization

### Phase 2 - Medium (2-3 hari):
- [ ] Add Virtual Scrolling dengan QuickGrid
- [ ] Lazy load gambar

### Phase 3 - Nice to Have (Optional):
- [ ] Add server-side pagination as fallback
- [ ] Consider static export untuk laporan

---

## ⚠️ Testing Checklist

Setelah implementasi:
1. **Bandwidth Monitor** - DevTools Network tab, check total data transfer
2. **Load Time** - Measure OnInitialized vs render time
3. **Scroll Performance** - Cek FPS saat scroll dengan 1000+ rows
4. **Print Functionality** - Pastikan print preview masih OK
5. **Responsive Test** - Mobile viewport (bandwidthnya lebih concern)

---

## 💡 Rekomendasi Tambahan

1. **Server-side Compression:**
   ```csharp
   // Di Startup/Program.cs
   services.AddResponseCompression(opts => {
	   opts.Providers.Add<GzipCompressionProvider>();
   });
   ```

2. **Enable Compression Middleware:**
   ```csharp
   app.UseResponseCompression();
   ```

3. **Blazor Circuit Isolation** (jika multi-user):
   - Limit concurrent users/circuits
   - Server memory vs bandwidth trade-off

4. **Monitor SignalR:**
   - Default Blazor Server limit ~100KB per message
   - Large data split menjadi multiple updates
   - Pertimbangkan WebAssembly untuk high-volume data if budget allows

---

## 🎓 Best Practices untuk Future Development

1. **Always extract inline styles** → Use CSS classes
2. **Virtualize large lists** → Default approach
3. **Pre-compute conditional data** → At API/service layer
4. **Lazy load non-critical assets** → Images, heavy components
5. **Profile before optimize** → Use DevTools, not guesswork
