using Microsoft.EntityFrameworkCore;
using TobimSystem.Data;
using TobimSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. إعداد قاعدة البيانات SQLite
builder.Services.AddDbContext<TobimDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tobim.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 2. إنشاء قاعدة البيانات والجداول تلقائياً عند بدء التشغيل
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TobimDbContext>();
    db.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// 3. عرض صفحة الواجهة الأمامية الإدارية مباشرة عند زيارة الموقع
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(@"
<!DOCTYPE html>
<html lang=""ar"" dir=""rtl"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>TOBIM — نظام الإدارة والمدفوعات</title>
    <link rel=""icon"" type=""image/jpeg"" href=""/TobimLogo.jpg"">
    <script src=""https://cdn.tailwindcss.com""></script>
    <script defer src=""https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js""></script>
    <link href=""https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700;800;900&display=swap"" rel=""stylesheet"">
    <style>
        body { font-family: 'Cairo', sans-serif; }
        [x-cloak] { display: none !important; }
        .glass-card { background: rgba(30, 41, 59, 0.7); backdrop-filter: blur(12px); border: 1px solid rgba(255, 255, 255, 0.08); }
        @keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        .animate-fade-in { animation: fadeIn 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards; }
    </style>
</head>
<body class=""bg-[#0f172a] text-gray-100 min-h-screen antialiased selection:bg-blue-600 selection:text-white"" x-data=""tobimApp()"">

    <!-- شاشة تسجيل الدخول -->
    <div x-show=""!isLoggedIn"" x-transition.opacity class=""fixed inset-0 z-50 flex items-center justify-center bg-[#090d16] p-4"">
        <div class=""glass-card w-full max-w-md p-8 rounded-3xl shadow-2xl border border-gray-800 animate-fade-in relative overflow-hidden"">
            <div class=""text-center mb-8"">
                <img src=""/TobimLogo.jpg"" class=""w-16 h-16 rounded-2xl mx-auto object-cover mb-4 border border-gray-700 shadow-md"" alt=""Logo"">
                <h1 class=""text-2xl font-black tracking-wider text-white"">TO BIM — تسجيل الدخول</h1>
                <p class=""text-xs text-gray-400 mt-1"">لوحة التحكم الإدارية المركزية</p>
            </div>
            <form @submit.prevent=""isLoggedIn = true"" class=""space-y-5"">
                <div>
                    <label class=""block text-xs font-semibold text-gray-400 mb-2"">الإيميل الإداري</label>
                    <input type=""email"" required value=""ahmed.tobim@gmail.com"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl px-4 py-3 text-sm text-white focus:outline-none focus:border-blue-500 transition"">
                </div>
                <div>
                    <label class=""block text-xs font-semibold text-gray-400 mb-2"">كلمة المرور</label>
                    <input type=""password"" required value=""••••••••"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl px-4 py-3 text-sm text-white focus:outline-none focus:border-blue-500 transition"">
                </div>
                <button type=""submit"" class=""w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-3.5 rounded-xl transition shadow-lg shadow-blue-600/30"">
                    دخول للنظام
                </button>
            </form>
        </div>
    </div>

    <!-- لوحة التحكم الرئيسية -->
    <div x-show=""isLoggedIn"" x-cloak class=""min-h-screen flex flex-col animate-fade-in"">
        <header class=""bg-[#111827]/90 backdrop-blur border-b border-gray-800 sticky top-0 z-40"">
            <div class=""max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-20 flex items-center justify-between"">
                <div class=""flex items-center gap-8"">
                    <div class=""flex items-center gap-3"">
                        <img src=""/TobimLogo.jpg"" class=""w-10 h-10 rounded-xl object-cover border border-gray-700"" alt=""Logo"">
                        <span class=""text-xl font-black tracking-wider text-white"">TO BIM</span>
                    </div>
                    <nav class=""hidden lg:flex items-center gap-2 font-semibold text-sm"">
                        <template x-for=""tab in tabs"">
                            <button @click=""currentTab = tab.id"" 
                                    :class=""currentTab === tab.id ? 'bg-blue-600 text-white shadow-md shadow-blue-600/30' : 'text-gray-400 hover:text-white hover:bg-gray-800/60'""
                                    class=""px-4 py-2.5 rounded-xl transition duration-250"">
                                <span x-text=""tab.name""></span>
                            </button>
                        </template>
                    </nav>
                </div>
                <div class=""flex items-center gap-4"">
                    <span class=""text-sm font-bold text-gray-300 bg-gray-800/80 px-3 py-1.5 rounded-lg border border-gray-700"">أحمد</span>
                    <button @click=""isLoggedIn = false"" class=""bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/30 px-4 py-2 rounded-xl text-sm font-bold transition"">
                        خروج
                    </button>
                </div>
            </div>
        </header>

        <main class=""flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8"">
            <!-- اشتراك جديد -->
            <div x-show=""currentTab === 'new'"" class=""glass-card p-6 sm:p-8 rounded-3xl border border-gray-800 shadow-xl"">
                <h2 class=""text-xl font-black text-white mb-6"">إضافة اشتراك جديد</h2>
                <div class=""grid grid-cols-1 md:grid-cols-3 gap-6 mb-6"">
                    <div>
                        <label class=""block text-xs font-semibold text-gray-400 mb-2"">القسم</label>
                        <select x-model=""form.department"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl p-3 text-sm text-white outline-none"">
                            <option>معماري</option>
                            <option>إنشائي</option>
                            <option>كورسات ودبلومات</option>
                        </select>
                    </div>
                    <div>
                        <label class=""block text-xs font-semibold text-gray-400 mb-2"">الراوند</label>
                        <select x-model=""form.round"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl p-3 text-sm text-white outline-none"">
                            <option>Round 27</option>
                            <option>Round 26</option>
                        </select>
                    </div>
                    <div>
                        <label class=""block text-xs font-semibold text-gray-400 mb-2"">سعر البيع</label>
                        <input type=""number"" x-model=""form.totalPrice"" placeholder=""12000"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl p-3 text-sm text-white outline-none"">
                    </div>
                </div>
                <div class=""grid grid-cols-1 md:grid-cols-3 gap-6 mb-6"">
                    <div>
                        <label class=""block text-xs font-semibold text-gray-400 mb-2"">اسم المشترك (ثلاثي)</label>
                        <input type=""text"" x-model=""form.fullName"" placeholder=""الاسم ثلاثي"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl p-3 text-sm text-white outline-none"">
                    </div>
                    <div>
                        <label class=""block text-xs font-semibold text-gray-400 mb-2"">موبايل المشترك</label>
                        <input type=""text"" x-model=""form.mobile"" placeholder=""01111111111"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl p-3 text-sm text-white outline-none"">
                    </div>
                    <div>
                        <label class=""block text-xs font-semibold text-gray-400 mb-2"">المدفوع</label>
                        <input type=""number"" x-model=""form.paidAmount"" placeholder=""9500"" class=""w-full bg-[#1e293b] border border-gray-700 rounded-xl p-3 text-sm text-white outline-none"">
                    </div>
                </div>
                <div class=""flex justify-end"">
                    <button @click=""saveSubscription()"" class=""bg-blue-600 hover:bg-blue-500 text-white font-bold px-8 py-3 rounded-xl transition shadow-lg shadow-blue-600/30 text-sm"">حفظ الاشتراك</button>
                </div>
            </div>

            <!-- اشتراكاتي -->
            <div x-show=""currentTab === 'subscriptions'"" class=""glass-card p-6 rounded-3xl border border-gray-800"">
                <h2 class=""text-lg font-black text-white mb-6"">سجل الاشتراكات</h2>
                <table class=""w-full text-right text-sm"">
                    <thead class=""text-gray-400 text-xs uppercase border-b border-gray-800"">
                        <tr>
                            <th class=""p-3"">الاسم</th>
                            <th class=""p-3"">الموبايل</th>
                            <th class=""p-3"">القسم / الراوند</th>
                            <th class=""p-3"">السعر</th>
                            <th class=""p-3"">المدفوع</th>
                            <th class=""p-3"">المتبقي</th>
                            <th class=""p-3"">الحالة</th>
                        </tr>
                    </thead>
                    <tbody class=""divide-y divide-gray-800"">
                        <template x-for=""sub in subscriptions"">
                            <tr class=""hover:bg-[#162032]/50 transition"">
                                <td class=""p-3 font-bold text-white"" x-text=""sub.fullName""></td>
                                <td class=""p-3 text-gray-300 font-mono"" x-text=""sub.mobile""></td>
                                <td class=""p-3 text-blue-400"" x-text=""sub.department + ' / ' + sub.round""></td>
                                <td class=""p-3 font-mono"" x-text=""sub.totalPrice + ' EGP'""></td>
                                <td class=""p-3 font-mono text-emerald-400"" x-text=""sub.paidAmount + ' EGP'""></td>
                                <td class=""p-3 font-mono text-amber-400"" x-text=""sub.remainingAmount + ' EGP'""></td>
                                <td class=""p-3"">
                                    <span :class=""sub.status === 'Full Pay' ? 'bg-emerald-500/20 text-emerald-400 border-emerald-500/30' : 'bg-amber-500/20 text-amber-400 border-amber-500/30'"" class=""px-3 py-1 rounded-full text-xs font-bold border"" x-text=""sub.status""></span>
                                </td>
                            </tr>
                        </template>
                    </tbody>
                </table>
            </div>

            <!-- كل المشتركين -->
            <div x-show=""currentTab === 'all'"" class=""glass-card p-6 rounded-3xl border border-gray-800"">
                <h2 class=""text-lg font-black text-white mb-2"">قاعدة بيانات المشتركين العامة</h2>
                <p class=""text-xs text-gray-400"">جميع المشتركين المسجلين في النظام عبر مختلف الأقسام.</p>
            </div>

            <!-- المستحق -->
            <div x-show=""currentTab === 'due'"" class=""glass-card p-6 rounded-3xl border border-gray-800"">
                <h2 class=""text-lg font-black text-white mb-2"">المبالغ المستحقة والمتبقية</h2>
                <p class=""text-xs text-gray-400"">متابعة الحسابات المعلقة للعملاء.</p>
            </div>

            <!-- طلباتي -->
            <div x-show=""currentTab === 'requests'"" class=""glass-card p-6 rounded-3xl border border-gray-800"">
                <h2 class=""text-lg font-black text-white mb-2"">متابعة الطلبات (IDs وفتح الكورسات)</h2>
                <p class=""text-xs text-gray-400"">حالة تنفيذ الطلبات الخاصة بالمشتركين.</p>
            </div>
        </main>
    </div>

    <script>
        function tobimApp() {
            return {
                isLoggedIn: true,
                currentTab: 'new',
                tabs: [
                    { id: 'new', name: 'اشتراك جديد' },
                    { id: 'subscriptions', name: 'اشتراكاتي' },
                    { id: 'all', name: 'كل المشتركين' },
                    { id: 'due', name: 'المستحق' },
                    { id: 'requests', name: 'طلباتي' }
                ],
                form: { department: 'معماري', round: 'Round 27', fullName: '', mobile: '', totalPrice: 0, paidAmount: 0 },
                subscriptions: [],
                async fetchSubs() {
                    let res = await fetch('/api/subscriptions');
                    this.subscriptions = await res.json();
                },
                async saveSubscription() {
                    let res = await fetch('/api/subscriptions', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(this.form)
                    });
                    if(res.ok) {
                        alert('تم حفظ الاشتراك بنجاح!');
                        this.form.fullName = '';
                        this.form.mobile = '';
                        this.fetchSubs();
                        this.currentTab = 'subscriptions';
                    }
                },
                init() { this.fetchSubs(); }
            }
        }
    </script>
</body>
</html>
");
});

app.Run();
