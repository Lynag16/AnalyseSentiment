# **ARI2: Création d'une application d'Analyse de Sentiment avec ML.NET**


## Introduction
----------------

Le but de ce TP est la création d'une application simple de prédiction de sentiment en utilisant ML.NET, un framework d'apprentissage automatique pour .NET.

## Prérequis
- .NET SDK .
- Visual Studio Code.
- Fichier de données contenant des commentaires avec des étiquettes de sentiments.

## Étapes pour la Construction
-------------------------------

## **1\. Télécharger les données**

Dans ce repository vous trouverez un fichier (commentdata.txt) contenant un jeu de données annoté avec des sentiments où chaque ligne contient un commentaire et un label de sentiment.

## **2\. Préparer l'environnement**

### a.  Vérifiez si .NET est installé :
```
    dotnet --version
```
### b.  Si dotnet n'est pas présent, installez le en suivant  :

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
## 4.  Ajoutez la dépendance ML.NET au projet :
```
    dotnet add package Microsoft.ML --version 2.0.0
``` 

Vérifiez que le fichier  'AnalyseSentiment.csproj 'contient cette ligne dans la section <ItemGroup> pour inclure ML.NET, sinon ajoutez la. 

```xml
    <PackageReference Include="Microsoft.ML" Version="2.0.0" />
```
## 5. Créez un dossier pour les données 

```
    mkdir Data
```
Copier le fichier de données que vous aviez téléchargé dans ce repertoire Data.

* * * * *

## 6 .On commence à construire le Projet Partie par Partie

### **a) Création de la Classe `SentimentData`**
**Ajoutez un nouveau fichier** dans le projet :
```bash
    touch SentimentData.cs
```
##### **Partie 1 : On crée un namespace et on déclare la classe pour définir les données**

```
using Microsoft.ML.Data;

namespace WebsiteCommentPredictor
{
    public class SentimentData
    {
      
    }
}

```


-   `namespace` : Permet de structurer le code.
-   `using Microsoft.ML.Data` : Nécessaire pour utiliser les annotations comme `[LoadColumn]`.

##### **Partie 2 : Ajouter les Colonnes**

```csharp

[LoadColumn(0)] 
public string? SentimentText { get; set; } // Texte du commentaire

[LoadColumn(1), ColumnName("Label")] 
public bool Sentiment { get; set; } // Label (1 = positif, 0 = négatif)
```

-   `[LoadColumn(x)]` : Lie une colonne spécifique du fichier de données.
-   `SentimentText` : Contient le texte du commentaire.
-   `Sentiment` : Contient le label (positif/négatif).

##### **Partie 3 : Prédictions**

```csharp
public class SentimentPrediction : SentimentData
{
    [ColumnName("PredictedLabel")] 
    public bool Prediction { get; set; }

    public float Probability { get; set; } 
    public float Score { get; set; } 
}
```


-   `SentimentPrediction` hérite de `SentimentData` pour réutiliser ses propriétés.
-   `Prediction`, `Probability` et `Score` : Données générées par le modèle.

* * * * *

### **b) Charger et Diviser les Données**

Le modèle doit lire et diviser les données avant de s'entraîner.

On doit modifier **Program.cs** :

**1 : Charger les Données**

Ajoutez cette fonction après `Main` :

```csharp
static DataOperationsCatalog.TrainTestData LoadData(MLContext mlContext, string dataPath)
{
    // Charger les données depuis le fichier texte
    IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(
        dataPath,
        hasHeader: false, // Pas d'en-tête dans les données
        separatorChar: '\t' 
    );

    // Diviser les données en train/test (80% / 20%)
    return mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
}
```
-   `LoadFromTextFile` : Charge les données depuis un fichier texte.
-   `TrainTestSplit` : Divise les données en ensembles d'entraînement et de test.

**2 : Appeler la Fonction dans `Main`**

Ajoutez ces lignes dans `Main` :

```csharp
string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "commentdata.txt");

// Création du contexte ML.NET
MLContext mlContext = new MLContext();

// Charger les données
var splitDataView = LoadData(mlContext, dataPath);
```

-   `MLContext` : Point d'entrée pour toutes les opérations ML.NET.
-   `dataPath` : Chemin du fichier de données.
-   `splitDataView` : Contient les ensembles d'entraînement et de test.

* * * * *

### **c) Construire et Entraîner le Modèle**

Nous devons configurer un pipeline pour entraîner le modèle.

Ajoutez cette fonction après `LoadData` :

```csharp
static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainData)
{
    // Pipeline de transformation des données
    var pipeline = mlContext.Transforms.Text.FeaturizeText(
            outputColumnName: "Features", // Convertir le texte en caractéristiques numériques
            inputColumnName: nameof(SentimentData.SentimentText)) // Colonne d'entrée
        .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
            labelColumnName: "Label", // Colonne cible
            featureColumnName: "Features")); // Caractéristiques

    // Entraîner le modèle
    return pipeline.Fit(trainData);
}
```

-   `FeaturizeText` : Transforme du texte brut en caractéristiques numériques.
-   `SdcaLogisticRegression` : Algorithme d'entraînement pour classification binaire.

1.  Appelez cette fonction dans `Main` :

```csharp
ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
```
* * * * *

### **d) Ajouter la Prédiction**

Cette fonction permet d'utiliser le modèle pour prédire le sentiment d'un texte.
Ajoutez cette fonction après `BuildAndTrainModel` :
```csharp

static void GetPredictionForReviewContent(MLContext mlContext, ITransformer model, string reviewContent)
{
    // Créer un moteur de prédiction
    var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

    // Préparer les données d'entrée
    var sampleStatement = new SentimentData
    {
        SentimentText = reviewContent
    };

    // Obtenir la prédiction
    var prediction = predictionEngine.Predict(sampleStatement);

    // Afficher le résultat
    Console.WriteLine($"Sentiment prédit : {(prediction.Prediction ? "Positif" : "Négatif")} " +
                      $"(Probabilité : {prediction.Probability:P2})");
}
```

Ajoutez cette boucle dans `Main` pour permettre plusieurs requêtes:

```csharp
Console.WriteLine("Entrez un commentaire pour analyser le sentiment (Appuyez sur Entrée pour quitter) :");
while (true)
{
    string userInput = Console.ReadLine();
    if (string.IsNullOrEmpty(userInput)) break;

    GetPredictionForReviewContent(mlContext, model, userInput);
}
```

* * * * *

### **e) Compiler et Exécuter**

1.  **Compilation** :

```bash
    dotnet build
```

2.  **Exécution** :

```bash
    dotnet run
```
* * * * *


\uD83C\uDF89 **Et un grand MERCI !**