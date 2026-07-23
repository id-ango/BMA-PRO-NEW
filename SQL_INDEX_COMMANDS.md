# SQL Index Creation - Copy & Paste Ready

## 🎯 Quick Copy-Paste SQL

Choose one based on your database type and copy the entire block.

---

## 1️⃣ SQL Server (Most Common)

```sql
-- SQL Server: Create unique index on DocNo
CREATE UNIQUE INDEX IX_CbTransHs_DocNo ON dbo.CbTransHs (DocNo)

-- Verify it was created
SELECT name, type_desc, is_unique 
FROM sys.indexes 
WHERE object_id = OBJECT_ID('dbo.CbTransHs')
ORDER BY create_date DESC
```

**Steps to execute**:
1. Open SQL Server Management Studio
2. Connect to your database
3. Open New Query
4. Copy-paste the SQL above
5. Click Execute (F5)
6. Look for "Command completed successfully"

---

## 2️⃣ PostgreSQL

```sql
-- PostgreSQL: Create unique index on DocNo
CREATE UNIQUE INDEX IX_CbTransHs_DocNo ON "CbTransHs" ("DocNo")

-- Verify it was created
SELECT indexname, indexdef 
FROM pg_indexes 
WHERE tablename = 'CbTransHs' 
ORDER BY indexname
```

**Steps to execute**:
1. Open pgAdmin or psql
2. Connect to your database
3. Open SQL Editor / New Query
4. Copy-paste the SQL above
5. Execute / Run
6. Look for "CREATE INDEX"

---

## 3️⃣ MySQL

```sql
-- MySQL: Create unique index on DocNo
ALTER TABLE CbTransHs ADD UNIQUE INDEX IX_CbTransHs_DocNo (DocNo)

-- Verify it was created
SHOW INDEX FROM CbTransHs
```

**Steps to execute**:
1. Open MySQL Workbench
2. Connect to your database
3. Open New Script
4. Copy-paste the SQL above
5. Click Execute / Run Query
6. Look for "0 rows affected" (index created)

---

## 4️⃣ If Table is Very Large (Optimization)

For tables with millions of rows, create index with ONLINE mode (SQL Server):

```sql
-- SQL Server: Create index online (doesn't lock table)
CREATE UNIQUE INDEX IX_CbTransHs_DocNo ON dbo.CbTransHs (DocNo) 
WITH (ONLINE = ON)

-- PostgreSQL: Create index concurrently (doesn't lock table)
CREATE UNIQUE INDEX CONCURRENTLY IX_CbTransHs_DocNo ON "CbTransHs" ("DocNo")

-- MySQL: Usually fast enough, but you can add algorithm
ALTER TABLE CbTransHs ADD UNIQUE INDEX IX_CbTransHs_DocNo (DocNo) 
ALGORITHM=INPLACE, LOCK=NONE
```

---

## ⚠️ If You Get Duplicate Error

If you get an error about duplicate values, run this first:

```sql
-- Find duplicates
SELECT DocNo, COUNT(*) as cnt 
FROM CbTransHs 
GROUP BY DocNo 
HAVING COUNT(*) > 1

-- Remove old duplicates (keep latest ID)
DELETE FROM CbTransHs 
WHERE CbTransHId NOT IN (
	SELECT MAX(CbTransHId) 
	FROM CbTransHs 
	GROUP BY DocNo
)

-- Then create the index
CREATE UNIQUE INDEX IX_CbTransHs_DocNo ON dbo.CbTransHs (DocNo)
```

---

## ✅ Verification Commands

After creating the index, run one of these to verify:

### SQL Server
```sql
SELECT 
	i.name AS IndexName,
	i.type_desc AS IndexType,
	i.is_unique AS IsUnique,
	CONVERT(DECIMAL(10,2), ISNULL(SUM(s.used_page_count) * 8 / 1024.0, 0)) AS SizeMB
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s 
	ON i.object_id = s.object_id 
	AND i.index_id = s.index_id 
	AND s.database_id = DB_ID()
WHERE OBJECT_ID('dbo.CbTransHs', 'U') = i.object_id
GROUP BY i.name, i.type_desc, i.is_unique
ORDER BY i.create_date DESC
```

### PostgreSQL
```sql
SELECT schemaname, tablename, indexname, indexdef
FROM pg_indexes
WHERE tablename = 'CbTransHs'
ORDER BY indexname DESC
```

### MySQL
```sql
SHOW INDEX FROM CbTransHs
DESCRIBE CbTransHs
```

---

## 🔍 Check Index Performance

After creating index, check if it's being used:

### SQL Server (Execution Plan)
```sql
-- Enable actual execution plan
SET STATISTICS IO ON
SET STATISTICS TIME ON

-- Run a typical search query
SELECT DocNo FROM CbTransHs WHERE DocNo LIKE 'COMP20250211%'

-- Review: "Seek" on index is good, "Scan" is bad
```

### PostgreSQL
```sql
EXPLAIN ANALYZE
SELECT DocNo FROM CbTransHs WHERE DocNo LIKE 'COMP20250211%'

-- Look for "Index Scan" in output
```

### MySQL
```sql
EXPLAIN
SELECT DocNo FROM CbTransHs WHERE DocNo LIKE 'COMP20250211%'

-- Look for "key: ix_CbTransHs_DocNo" in output
```

---

## 🛠️ Troubleshooting Commands

### Check current indexes on table
```sql
-- SQL Server
SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.CbTransHs')

-- PostgreSQL
SELECT indexname FROM pg_indexes WHERE tablename = 'CbTransHs'

-- MySQL
SHOW INDEX FROM CbTransHs
```

### Check index fragmentation (SQL Server only)
```sql
SELECT 
	OBJECT_NAME(ps.object_id) AS TableName,
	i.name AS IndexName,
	ps.avg_fragmentation_in_percent AS FragmentationPercent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ps
INNER JOIN sys.indexes i ON ps.object_id = i.object_id 
	AND ps.index_id = i.index_id
WHERE OBJECT_NAME(ps.object_id) = 'CbTransHs'
ORDER BY ps.avg_fragmentation_in_percent DESC
```

### Rebuild index if fragmented (SQL Server)
```sql
-- Rebuild if fragmentation > 10%
ALTER INDEX IX_CbTransHs_DocNo ON dbo.CbTransHs REBUILD

-- Reorganize if fragmentation 5-10%
ALTER INDEX IX_CbTransHs_DocNo ON dbo.CbTransHs REORGANIZE

-- Verify after rebuild
SELECT avg_fragmentation_in_percent
FROM sys.dm_db_index_physical_stats(DB_ID(), OBJECT_ID('dbo.CbTransHs'), NULL, NULL, 'LIMITED')
WHERE index_id > 0
```

---

## 📝 Before/After Comparison

### Before (Without Index)
```
Query: SELECT DocNo FROM CbTransHs WHERE DocNo LIKE 'COMP20250211%'
Execution Time: 5-30+ seconds
Operation: Table Scan (examines all rows)
Result: ❌ Timeout
```

### After (With Index)
```
Query: SELECT DocNo FROM CbTransHs WHERE DocNo LIKE 'COMP20250211%'
Execution Time: 10-100 milliseconds
Operation: Index Seek (jumps to matching rows)
Result: ✅ Success
```

---

## 🎯 Step-by-Step Execution

### Step 1: Open Database Tool
- SQL Server → SQL Server Management Studio
- PostgreSQL → pgAdmin or psql terminal
- MySQL → MySQL Workbench

### Step 2: Connect to Database
- Select your database server
- Connect with login credentials
- Open New Query / SQL Script

### Step 3: Copy SQL
- Select SQL statement for your database from above
- Copy entire statement (including comment lines)

### Step 4: Paste & Execute
- Paste into query window
- Highlight all code (Ctrl+A)
- Press Execute (F5) or Run Query
- Review results - should see "Command succeeded"

### Step 5: Verify
- Run verification command from above
- Should see IX_CbTransHs_DocNo in results
- Index size typically < 5-10% of table size

---

## ⏱️ Expected Duration

| Database | Table Size | Execution Time |
|----------|-----------|-----------------|
| Small (< 100K rows) | 100 MB | < 1 second |
| Medium (100K-1M rows) | 1-2 GB | 5-15 seconds |
| Large (1M-10M rows) | 2-10 GB | 15-60 seconds |
| Huge (> 10M rows) | > 10 GB | 1-5 minutes |

---

## ✨ That's It!

After running one of the SQL commands above:
1. Index is created
2. App will run 50-300x faster for document sequence queries
3. Timeouts will be eliminated
4. Users can save transactions successfully

**Questions?** See the detailed guides in the documentation folder.

