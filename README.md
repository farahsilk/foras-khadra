# English is Below
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/c1f1ec52-89b0-4201-ab44-4bfbc9cab5e0" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/e92f6e93-a2b8-49b6-a6c8-bdc44c375ba7" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/1fc18fa8-8740-4616-b67c-3c9b630b422f" />


# فرص خضراء — مساعد تحليلات ذكي بالذكاء الاصطناعي

هذا المشروع هو تطبيق ويب داخلي لفريق **فرص خضراء**، وليس موجّهًا للمستخدمين النهائيين مباشرة.

يساعد التطبيق الفريق على فهم بيانات الفرص والمنشورات والتفاعل من خلال لوحة تحكم بسيطة ومساعد ذكي يمكنه الإجابة على الأسئلة بناءً على البيانات الموجودة في المشروع.

تم بناء هذا المشروع ضمن مهمة:

**Foras Khadra Artificial Intelligence Task 2026**

---

## فكرة المشروع

منصة فرص خضراء تنشر فرصًا ومنحًا وبرامج تدريبية للشباب.
فكرة هذا المشروع هي استخدام الذكاء الاصطناعي لمساعدة الفريق على تحليل البيانات وفهم أداء المحتوى بشكل أفضل.

يتكوّن المشروع من جزأين رئيسيين:

1. **لوحة تحكم تحليلية**
   تعرض أرقامًا وإحصائيات مثل:

   * عدد الفرص الكلي
   * عدد الفرص المفتوحة والمغلقة
   * أنواع الفرص
   * عدد الزوار شهريًا
   * التفاعل حسب المنصة
   * أكثر الفرص مشاهدة

2. **مساعد ذكي**
   يسمح للفريق بطرح أسئلة باللغة العربية أو الإنجليزية، مثل:

   * ما هي أكثر الفرص مشاهدة؟
   * ما أنواع الفرص التي تحصل على تفاعل أكبر؟
   * ما الفرص المفتوحة حاليًا؟
   * ما الأسئلة التي يطرحها المتابعون كثيرًا؟
   * اقترح أفكارًا لمنشورات عن فرصة معينة.
   * ما نوع المحتوى الذي يجب نشره أكثر؟

المساعد الذكي يستخدم بيانات المشروع كمرجع، ثم يعطي إجابات عملية ومفيدة بناءً عليها.

---

## لماذا يناسب هذا المشروع المهمة؟

المهمة تطلب بناء ميزة صغيرة مدعومة بالذكاء الاصطناعي ومتصلة بتطبيق ويب، وتخدم سياق منصة فرص خضراء.

هذا المشروع يحقق المطلوب لأنه يحتوي على:

* ميزة ذكاء اصطناعي فعلية باستخدام Gemini API.
* تطبيق ويب يحتوي على واجهة أمامية وخلفية.
* بيانات تجريبية Mock Data.
* لوحة تحكم تعرض تحليلات مفيدة.
* مساعد ذكي يجيب على الأسئلة بناءً على البيانات.
* ربط واضح بين الواجهة، الخادم، البيانات، وخدمة الذكاء الاصطناعي.

كما أن المهمة تسمح باستخدام بيانات تجريبية، لذلك البيانات الموجودة في المشروع هي بيانات Mock/Synthetic لأغراض العرض والتجربة.

---

## التقنيات المستخدمة

| الجزء            | التقنية               |
| ---------------- | --------------------- |
| إطار العمل       | ASP.NET Core 8 MVC    |
| الواجهة          | Razor Views `.cshtml` |
| التصميم          | HTML, CSS, JavaScript |
| الرسوم البيانية  | Chart.js              |
| الخادم الخلفي    | C# / ASP.NET Core     |
| الذكاء الاصطناعي | Gemini API            |
| تخزين البيانات   | ملف JSON              |
| التعامل مع JSON  | System.Text.Json      |

المشروع مبني كتطبيق واحد باستخدام ASP.NET Core MVC، ولا يحتاج إلى React أو مشروع Frontend منفصل.

---

## أداة الذكاء الاصطناعي المستخدمة

يستخدم المشروع **Gemini API** كمزود للذكاء الاصطناعي.

المشروع لا يقوم بتدريب نموذج ذكاء اصطناعي خاص من الصفر، لأن البيانات التجريبية ليست كافية لتدريب نموذج مخصص بشكل مفيد.

بدلًا من ذلك، يتم إرسال ملخص من بيانات المشروع مع سؤال المستخدم إلى نموذج Gemini، ثم يرجع النموذج بإجابة مفهومة باللغة الطبيعية.

هذا مناسب للمشروع لأن:

* التنفيذ أبسط وأسرع.
* لا يحتاج إلى تدريب نموذج.
* يستطيع المساعد الإجابة بالعربية والإنجليزية.
* الإجابات تكون مرتبطة بالبيانات الموجودة في المشروع.
* هذا الأسلوب عملي ومناسب لتطبيق ويب صغير.

---

## كيف يعمل المساعد الذكي؟

المساعد لا يعطي إجابات عشوائية.
هو يعمل بهذه الطريقة:

```text
سؤال المستخدم
     ↓
ChatController يستقبل السؤال
     ↓
DataService يقرأ ويلخص بيانات JSON
     ↓
GeminiProvider يرسل السؤال + ملخص البيانات إلى Gemini API
     ↓
Gemini يرجع الإجابة
     ↓
تظهر الإجابة للمستخدم داخل صفحة الشات
```

إذا لم تكن المعلومة موجودة في البيانات، يجب أن يوضح المساعد أن البيانات غير كافية بدلًا من اختراع إجابة.

---

## مسار البيانات داخل المشروع

```text
Data/foras_khadra_data.json
        ↓
DataService
        ↓
Controllers
   ├── AnalyticsController
   └── ChatController
        ↓
Razor Views + JavaScript
        ↓
Dashboard + AI Chat
```

لوحة التحكم تستخدم بيانات JSON مباشرة لعرض الإحصائيات.

أما الشات فيستخدم بيانات JSON كمرجع قبل إرسال السؤال إلى Gemini.

---

## هيكل المشروع

```text
ForasKhadra.API/
├── Controllers/
│   ├── HomeController.cs
│   ├── AnalyticsController.cs
│   └── ChatController.cs
│
├── Services/
│   ├── DataService.cs
│   ├── ChatService.cs
│   ├── ILlmProvider.cs
│   └── Providers/
│       └── GeminiProvider.cs
│
├── Models/
│   └── Data and DTO classes
│
├── Data/
│   └── foras_khadra_data.json
│
├── Views/
│   ├── Home/
│   │   ├── Dashboard.cshtml
│   │   └── Chat.cshtml
│   └── Shared/
│       └── _Layout.cshtml
│
├── wwwroot/
│   ├── css/
│   └── js/
│
├── Program.cs
├── appsettings.json
└── README.md
```

---

## البيانات المستخدمة

يحتوي الملف التالي على بيانات تجريبية:

```text
Data/foras_khadra_data.json
```

البيانات تشمل أمثلة على:

* الفرص
* أنواع الفرص
* الدول
* المواعيد النهائية
* عدد المشاهدات
* منشورات منصات التواصل
* الإعجابات
* التعليقات
* المشاركات
* الوصول Reach
* الزوار الشهريين
* أمثلة على ملفات مستخدمين

هذه البيانات ليست بيانات خاصة، وليست مأخوذة من حسابات خاصة.
هي بيانات تجريبية تم إنشاؤها فقط لعرض فكرة المشروع واختبار المساعد الذكي.

استخدام هذه البيانات مناسب للمهمة، لأن نص المهمة يسمح باستخدام Mock Data.

---

## طريقة تشغيل المشروع

### 1. تثبيت .NET 8 SDK

يجب أن يكون لديك .NET 8 SDK مثبت على الجهاز.

---

### 2. تحميل المشروع من GitHub

```bash
git clone https://github.com/farahsilk/foras-khadra.git
cd foras-khadra/ForasKhadra.API
```

---

### 3. إضافة مفتاح Gemini API بطريقة آمنة

لا تضع مفتاح API الحقيقي داخل ملف:

```text
appsettings.json
```

استخدم `dotnet user-secrets` بدلًا من ذلك:

```bash
dotnet user-secrets init
dotnet user-secrets set "Llm:ActiveProvider" "Gemini"
dotnet user-secrets set "Llm:Gemini:ApiKey" "YOUR_GEMINI_API_KEY"
dotnet user-secrets set "Llm:Gemini:Model" "gemini-2.5-flash"
```

استبدل:

```text
YOUR_GEMINI_API_KEY
```

بمفتاح Gemini الحقيقي الخاص بك.

---

### 4. تشغيل المشروع

```bash
dotnet run
```

---

### 5. فتح التطبيق في المتصفح

بعد تشغيل المشروع، سيظهر رابط في التيرمنال مثل:

```text
https://localhost:7090
```

افتح الرابط في المتصفح.

الصفحات الرئيسية:

```text
/           لوحة التحكم
/Home/Chat  المساعد الذكي
```

لوحة التحكم يمكن أن تعمل بدون مفتاح API.
أما المساعد الذكي فيحتاج إلى مفتاح Gemini API صحيح.

---

## أمثلة على أسئلة يمكن طرحها على المساعد

```text
ما هي أكثر الفرص مشاهدة؟
```

```text
ما هي الفرص المفتوحة حاليًا؟
```

```text
ما نوع الفرص الذي يحصل على أعلى تفاعل؟
```

```text
اقترح أفكار منشورات لفرصة تدريبية.
```

```text
Which platform has the best engagement?
```

```text
Show me the most popular open opportunities.
```

---

## API Endpoints

| Method | Route                              | الوظيفة                      |
| ------ | ---------------------------------- | ---------------------------- |
| GET    | `/api/analytics/overview`          | إحصائيات عامة للوحة التحكم   |
| GET    | `/api/analytics/visitors`          | الزوار الشهريون              |
| GET    | `/api/analytics/by-type`           | عدد الفرص حسب النوع          |
| GET    | `/api/analytics/engagement`        | التفاعل حسب المنصة           |
| GET    | `/api/analytics/top-opportunities` | أكثر الفرص مشاهدة            |
| POST   | `/api/chat/message`                | إرسال سؤال إلى المساعد الذكي |

---

## تحسينات مستقبلية ممكنة

يمكن تطوير المشروع مستقبلًا بإضافة:

* بيانات حقيقية من لوحة تحكم المنصة.
* بحث ذكي Semantic Search عن الفرص.
* نظام توصيات للمستخدمين.
* تسجيل دخول للفريق الداخلي.
* قاعدة بيانات بدل ملف JSON.
* نشر Live Demo.
* دعم مزودي ذكاء اصطناعي إضافيين بجانب Gemini.

---

## الخلاصة

هذا المشروع يعرض ميزة عملية مدعومة بالذكاء الاصطناعي لمنصة فرص خضراء.

يجمع المشروع بين:

* تطبيق ويب
* لوحة تحكم
* Backend بلغة C#
* بيانات تجريبية
* Gemini API
* مساعد ذكي يجيب بناءً على البيانات

الهدف هو إظهار كيف يمكن للذكاء الاصطناعي مساعدة فريق فرص خضراء على فهم البيانات، تحسين المحتوى، واتخاذ قرارات أفضل تخدم الشباب الباحثين عن الفرص الخضراء.
---
# Foras Khadra — AI Analytics Assistant

An internal AI-powered web application for the **Foras Khadra (فرص خضراء)** team.

The app helps the team understand opportunity and social media data through a simple dashboard and an AI assistant. The assistant can answer questions about opportunities, engagement, visitors, popular content, and user interests using mock data that represents the structure of a real opportunity platform.

This project was built for the **Foras Khadra Artificial Intelligence Task 2026**.

---

## Project Idea

Foras Khadra publishes opportunities and scholarships for young people. The goal of this project is to show how AI can help the platform team analyze data and make better decisions.

The project contains two main parts:

1. **Analytics Dashboard**
   Shows useful statistics such as:

   * Total opportunities
   * Open and closed opportunities
   * Opportunities by type
   * Monthly visitors
   * Engagement by platform
   * Most viewed opportunities

2. **AI Assistant**
   Allows the team to ask questions in Arabic or English, such as:

   * What opportunities are currently open?
   * Which type of opportunity performs best?
   * What are users asking about?
   * Which posts have the highest engagement?
   * Suggest caption ideas for a scholarship post.
   * Recommend what type of opportunity should be posted more often.

The assistant uses the project data as context and gives practical answers based on that data.

---

## Why This Project Fits the Task

The task asks for a small **AI-powered feature** integrated into a web application that serves the context of Foras Khadra.

This project fits the requirements because it includes:

* A real AI feature using an external LLM API.
* A web interface connected to a backend.
* Mock opportunity and social media data.
* A practical feature that can help the Foras Khadra team understand their content and improve the platform.
* Clear integration between the frontend, backend, data layer, and AI service.

The task allows using mock data, so the dataset in this project is synthetic and created for demonstration purposes.

---

## Tech Stack

| Layer         | Technology                                   |
| ------------- | -------------------------------------------- |
| Framework     | ASP.NET Core 8 MVC                           |
| UI            | Razor Views `.cshtml`, HTML, CSS, JavaScript |
| Charts        | Chart.js                                     |
| Backend       | C# / ASP.NET Core                            |
| AI Provider   | Gemini API                                   |
| Data Storage  | JSON file loaded into memory                 |
| JSON Handling | System.Text.Json                             |

The project is built as one ASP.NET Core MVC application. There is no separate React app required.

---

## AI Tool Used

This project uses the **Gemini API** as the AI provider.

The project does not train a custom AI model. Instead, it sends a summarized version of the project data to the AI model together with the user's question.

This is better for a small project because:

* The dataset is not large enough to train a custom model.
* Using an existing LLM is faster and more reliable.
* The AI can answer in natural language.
* The assistant can work in both Arabic and English.
* The answers stay related to the provided data.

---

## How the AI Assistant Works

The AI assistant does not answer randomly. It follows this flow:

```text
User question
     ↓
ChatController receives the question
     ↓
DataService loads and summarizes the JSON data
     ↓
GeminiProvider sends the question + data context to Gemini API
     ↓
Gemini returns an answer
     ↓
The answer is shown in the web interface
```

The assistant is designed to answer based on the available dataset. If the data does not contain enough information, the assistant should say that the information is not available instead of inventing an answer.

---

## Data Flow

```text
Data/foras_khadra_data.json
        ↓
DataService
        ↓
Controllers
   ├── AnalyticsController
   └── ChatController
        ↓
Razor Views + JavaScript
        ↓
Dashboard and AI Chat
```

The dashboard uses the JSON data directly.

The chatbot uses the JSON data as context for Gemini.

---

## Project Structure

```text
ForasKhadra.API/
├── Controllers/
│   ├── HomeController.cs
│   ├── AnalyticsController.cs
│   └── ChatController.cs
│
├── Services/
│   ├── DataService.cs
│   ├── ChatService.cs
│   ├── ILlmProvider.cs
│   └── Providers/
│       └── GeminiProvider.cs
│
├── Models/
│   └── Data and DTO classes
│
├── Data/
│   └── foras_khadra_data.json
│
├── Views/
│   ├── Home/
│   │   ├── Dashboard.cshtml
│   │   └── Chat.cshtml
│   └── Shared/
│       └── _Layout.cshtml
│
├── wwwroot/
│   ├── css/
│   └── js/
│
├── Program.cs
├── appsettings.json
└── README.md
```

---

## About the Data

The file `Data/foras_khadra_data.json` contains mock data created for this project.

The dataset includes examples of:

* Opportunities
* Opportunity types
* Countries
* Deadlines
* Page views
* Social media posts
* Likes
* Comments
* Shares
* Reach
* Monthly visitors
* User profile examples

The data is not private and is not scraped from private accounts. It is synthetic/mock data used only to demonstrate the AI feature.

This follows the task instructions, which allow using mock opportunity data.

---

## Running the Project

### 1. Install .NET 8 SDK

Make sure you have the .NET 8 SDK installed.

### 2. Clone the repository

```bash
git clone https://github.com/farahsilk/foras-khadra.git
cd foras-khadra/ForasKhadra.API
```

### 3. Set the Gemini API key safely

Do not write the API key directly inside `appsettings.json`.

Use user secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "Llm:ActiveProvider" "Gemini"
dotnet user-secrets set "Llm:Gemini:ApiKey" "YOUR_GEMINI_API_KEY"
dotnet user-secrets set "Llm:Gemini:Model" "gemini-2.5-flash"
```

### 4. Run the project

```bash
dotnet run
```

### 5. Open the app in the browser

Use the URL shown in the terminal, for example:

```text
https://localhost:7090
```

Main pages:

```text
/           Dashboard
/Home/Chat  AI Assistant
```

The dashboard can work without an API key.
The AI assistant needs a valid Gemini API key.

---

## Example Questions for the AI Assistant

You can ask:

```text
What are the most popular open opportunities?
```

```text
ما هي أكثر أنواع الفرص تفاعلاً؟
```

```text
Suggest captions for an internship opportunity.
```

```text
Which platform has the best engagement?
```

```text
ما الفرص المناسبة للطلاب المهتمين بالطاقة المتجددة؟
```

---

## API Endpoints

| Method | Route                              | Purpose                              |
| ------ | ---------------------------------- | ------------------------------------ |
| GET    | `/api/analytics/overview`          | Main dashboard numbers               |
| GET    | `/api/analytics/visitors`          | Monthly visitors                     |
| GET    | `/api/analytics/by-type`           | Opportunities grouped by type        |
| GET    | `/api/analytics/engagement`        | Engagement by platform               |
| GET    | `/api/analytics/top-opportunities` | Most viewed opportunities            |
| POST   | `/api/chat/message`                | Sends a question to the AI assistant |

---

## Future Improvements

Possible improvements include:

* Adding real admin-exported social media data.
* Adding semantic search for opportunities.
* Adding user-based recommendations.
* Adding authentication for internal team members.
* Adding a database instead of a JSON file.
* Adding a live demo deployment.
* Supporting more AI providers in addition to Gemini.

---

## Summary

This project demonstrates a practical AI-powered feature for Foras Khadra.

It combines:

* A web dashboard
* A backend service
* Mock opportunity and social media data
* Gemini AI integration
* A chatbot that answers questions based on platform data

The goal is to show how AI can help the Foras Khadra team understand their data, improve content decisions, and support young people in finding relevant green opportunities.
