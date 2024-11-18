# **ARI2: Création d'une Application d'Analyse de Sentiment avec ML.NET**

## **Introduction**
Ce projet consiste à créer une application web interactive permettant d'analyser le sentiment d'un texte en utilisant ML.NET, un framework d'apprentissage automatique pour .NET. Cette application prend un commentaire utilisateur, prédit si le sentiment est positif ou négatif.

---

## **Prérequis**
Avant de commencer, assurez-vous que les éléments suivants sont installés et configurés sur votre machine :

- **.NET SDK**  
- **Visual Studio Code**  
- **Fichier de données** contenant des commentaires étiquetés avec des sentiments (positif/négatif). Le dataset est fourni dans le fichier `commentdata.txt` dans ce repository.

---

## **Étapes pour la Construction**

## 1. Télécharger les Données
Le fichier `commentdata.txt` contient des commentaires annotés avec des sentiments, où chaque ligne correspond à un commentaire et un label de sentiment (1 = positif, 0 = négatif).  
Chemin du fichier : Data/commentdata.txt
- **Origine du dataset** : [Microsoft Learn](https://learn.microsoft.com/).
---

## 2. Préparer l'Environnement

#### a. Vérifiez si .NET est installé
Utilisez la commande suivante dans votre terminal pour vérifier la version de .NET :
```bash
dotnet --version
```
#### b.  Si dotnet n'est pas présent, installez le en suivant  :

```
    wget https://dot.net/v1/dotnet-install.sh
```

```
    bash dotnet-install.sh --channel 8.0 --install-dir ~/dotnet
```
```
    export PATH=$PATH:~/dotnet
```
```bash
    source ~/.bashrc
```
## 3.  Allez sur VSCode et créez un nouveau projet dotnet:
```bash
    dotnet new webapi -n AnalyseSentiment
```
```
    cd AnalyseSentiment
```

## 4.  Ajoutez les dépendances :
```
    dotnet add package Microsoft.ML --version 2.0.0
    dotnet add package Microsoft.ML.AspNetCore
``` 

Vérifiez que le fichier  'AnalyseSentiment.csproj 'contient cette ligne dans la section <ItemGroup> pour inclure ML.NET, sinon ajoutez la. 

```xml
    <PackageReference Include="Microsoft.ML" Version="2.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="3.0.0" />
```
* * * * *

## 5. Structuter le Projet et ajouter les classes 
Naviguez dans le dossier du projet et organisez les fichiers nécessaires, en suivant cette structure.

```bash

AnalyseSentiment/ 
│
├── Data/                        
│   └── commentdata.txt          
│
├── Models/                      
│   ├── SentimentData.cs        
│
├── Controllers/                       
│   ├── SentimentControllers.cs     
│ 
│
├── Services/                   
│   ├── SentimentService.cs      
│
├── wwwroot/                     
│   ├── css/
│   │   └── styles.css     
│   ├── js/
│   │   └── app.js     
│   │   └── index.html         
│
├── Properties/                  
│   └── launchSettings.json      
│
├── AnalyseSentiment.csproj      
│
├── Program.cs                   
│
└── appsettings.json             

```

##  Etapes:

**1. Copier le fichier des donées dans Data**

**2. Créer une classe SentimentData.cs dans Models pour définir le schéma des données.**

```csharp
using Microsoft.ML.Data;


namespace AnalyseSentiment.Models
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string? SentimentText { get; set; }

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment { get; set; }
    }

    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
```
- SentimentData : Cette classe représente les données que nous allons utiliser pour l'entraînement de notre modèle ML. Elle contient les colonnes de texte à analyser et la colonne étiquetée de "Sentiment".
- SentimentPrediction : Cette classe hérite de SentimentData et représente les résultats de la prédiction effectuée par le modèle après l'entraînement.


**3. Créer SentimentService.cs pour gérer le modèle ML.NET.**

a. Importer les Bibliothèques

```csharp
using Microsoft.ML;
using Microsoft.ML.Data;
using AnalyseSentiment.Models;
```
- *Microsoft.ML* : Fournit les classes nécessaires pour créer et entraîner un modèle.
- *Microsoft.ML.Data* : Gère les annotations de colonnes et les prédictions.
- *AnalyseSentiment.Models* : Inclut les définitions des classes SentimentData et SentimentPrediction.

b. Définir la Classe

```csharp
namespace AnalyseSentiment.Services
{
    public class SentimentService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly string _yelpDataPath;

    }
}

```

- MLContext : Sert à configurer le pipeline de données et les algorithmes de machine learning..
- ITransformer : Représente le modèle entraîné.
- _yelpDataPath : Définit le chemin du fichier contenant les données d'entraînement.
  
c. Initialisation

Ajouter un constructeur pour initialiser le contexte de ML.NET et le chemin des données : :

```csharp
public SentimentService()
{
    _mlContext = new MLContext(); // Initialisation du contexte ML
    _yelpDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "commentdata.txt"); 
    _model = BuildAndTrainModel(); 
}

```

d. Créer le Modèle
Screenshot from 2024-11-18 10-35-37
Ajouter la méthode pour construire et entraîner le modèle

```csharp
        public ITransformer BuildAndTrainModel()
        {
            
        }
```

e. Charger et Diviser les Données : 

```csharp
            var dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(_yelpDataPath, hasHeader: false, separatorChar: '\t');
            var splitDataView = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
```
- splitDataView : Division des données en deux ensembles pour l'entraînement et l'évaluation.
            
f. Définition du Pipeline :
Ajouter le pipeline qui transforme le texte en vecteurs et applique un modèle de régression logistique.

```csharp
            var estimator = _mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
```
- FeaturizeText : Convertit les données textuelles brutes en vecteurs numériques.
- SdcaLogisticRegression : Algorithme utilisé pour entraîner un modèle de classification binaire.

g. Retourner le Modèle
Après avoir configuré le pipeline, entraîner le modèle et retourner le resultat.
```csharp
 return estimator.Fit(splitDataView.TrainSet);
```

h. Prediction 
Ajouter la méthode permettant d’effectuer des prédictions pour de nouvelles données textuelles
```csharp
        public SentimentPrediction GetPredictionForReview(string reviewContent)
        {          
        }
```
i. Créer un moteur de prédiction pour appliquer le modèle.
```csharp
var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
```
j. Préparer les données d'entrée à prédire en les encapsulant dans un objet SentimentData.
```csharp
var sampleStatement = new SentimentData { SentimentText = reviewContent };
```

k. Effectuer une prédiction
```csharp
return predictionEngine.Predict(sampleStatement);
```

**4. Implémenter SentimentController.cs**

a. Importer les Bibliothèques
```csharp
using Microsoft.AspNetCore.Mvc;
using AnalyseSentiment.Models;
using AnalyseSentiment.Services;
```
- Microsoft.AspNetCore.Mvc : Contient les classes nécessaires pour créer des contrôleurs Web dans une API ASP.NET Core.
- AnalyseSentiment.Models : Définit les classes utilisées pour représenter les données et les prédictions (e.g., SentimentData et SentimentPrediction).
- AnalyseSentiment.Services : Fournit les services de prédiction de sentiments (SentimentService).

b. Définir la Classe et Configurer le Contrôleur
```csharp

namespace AnalyseSentiment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentController : ControllerBase
    {
        private readonly SentimentService _SentimentService;

    }
}

```
- [Route("api/[controller]")] : Définit le chemin d'accès aux routes de ce contrôleur. 
- [ApiController] : Indique que cette classe est un contrôleur API.
- SentimentService : Service injecté pour fournir les fonctionnalités de prédiction.
  
c. Injecter le Service dans le Constructeur
```csharp
public SentimentController(SentimentService SentimentService)
{
    _SentimentService = SentimentService;
}
```

- _SentimentService est utilisé pour appeler les méthodes du service.
  
d. Ajout de la Méthode de Prédiction
```csharp
[HttpPost("predict")]
public ActionResult<SentimentPrediction> PredictSentiment([FromBody] string reviewContent)
{
    var prediction = _SentimentService.GetPredictionForReview(reviewContent);
    return Ok(new 
    { 
        Sentiment = prediction.Prediction ? "Positive" : "Negative",
        Probability = prediction.Probability 
    });
}
```
- [HttpPost("predict")] : Spécifie que cette méthode répond à des requêtes POST sur la route /api/sentiment/predict.
- [FromBody] string reviewContent : Indique que la donnée d'entrée est fournie dans le corps de la requête.
- _SentimentService.GetPredictionForReview(reviewContent) : Appelle le service pour obtenir une prédiction basée sur le texte fourni.
- Ok() : Retourne un code HTTP 200 avec un objet JSON contenant :
    - Sentiment : Indique si le sentiment est positif ou négatif.
    - Probability : Probabilité associée à la prédiction.


**5. Modifier Program.cs**

a. Importer les Bibliothèques
```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AnalyseSentiment.Services;
```
- Microsoft.AspNetCore.Builder : Contient des classes pour configurer l'application, y compris les middlewares et la gestion des requêtes HTTP.
- Microsoft.Extensions.DependencyInjection : Permet l'injection de dépendances pour gérer les services dans l'application.
- Microsoft.Extensions.Hosting : Fournit des services pour gérer le cycle de vie de l'application.
- AnalyseSentiment.Services : Contient le service de prédiction SentimentService.
b. Créer le Builder
```csharp
var builder = WebApplication.CreateBuilder(args);
```
WebApplication.CreateBuilder(args) : Initialise un constructeur pour l'application ASP.NET Core.

c. Ajouter les Services
```csharp
builder.Services.AddSingleton<SentimentService>();  
builder.Services.AddControllers();  
```
- builder.Services.AddSingleton<SentimentService>() : Enregistre le service SentimentService comme un service singleton dans le conteneur d'injection de dépendance. Cela signifie que le même service sera utilisé à travers l'application.
- builder.Services.AddControllers() : Enregistre les contrôleurs MVC (ici, SentimentController) dans l'application. Cela permet d'utiliser des API REST basées sur des contrôleurs.
d. Construire l'Application
```csharp
var app = builder.Build();
```
- builder.Build() : Construit l'application après avoir configuré les services. À ce stade, l'application est prête à être configurée et exécutée.

e. Configuration du Pipeline HTTP
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
```
app.Environment.IsDevelopment() : Vérifie si l'application est en mode développement.
app.UseDeveloperExceptionPage() : Active une page d'erreur détaillée pour aider au débogage lorsque l'application est en développement.

f. Routing et Mapping des Contrôleurs
```csharp
app.UseRouting();
```
app.UseRouting() : Active le routage des requêtes HTTP en fonction des routes définies dans l'application.

```csharp
app.MapControllers();
```
app.MapControllers() : Mappe toutes les routes des contrôleurs API définis dans l'application, comme celles du SentimentController, permettant ainsi aux requêtes HTTP de correspondre aux actions des contrôleurs.

g. Exécution de l'Application
```csharp
app.Run();
```
app.Run() : Démarre l'application et commence à écouter les requêtes HTTP.



---

### **6. Compiler et Exécuter**  
- Compilez le projet avec `dotnet build` pour vérifier les erreurs.  
- Lancez l'application avec `dotnet run`.  

Screenshot from 2024-11-18 10-35-37

### **7. Tester**  
```bash
curl -X POST http://localhost:5121/api/sentiment/predict \
-H "Content-Type: application/json" \
-d "\"I love this product! It's amazing.\""

```
![image](https://github.com/user-attachments/assets/66c928c8-cb2c-4fda-9e8c-ec81f587692d)


**Et un grand MERCI !**
