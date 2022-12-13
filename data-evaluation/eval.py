import pandas as pd
from sklearn import linear_model
from sklearn.neural_network import MLPRegressor
from sklearn import svm

# Get the full set of features.
def getAllDice(filename: str):
    features = set()
    with open(filename,'r') as f:
        for line in f.readlines():
            tokens = line.split(',')
            features.add(tokens[0])
    return features

# Loads the data from the CSV file and parses it.
# This data is already randomized.
def loadData(filename:str, featureSet: set):
    results = pd.DataFrame(columns = list(featureSet) + ['Win Rate'])
    with open(filename,'r') as f:
        for line in f.readlines():
            tokens = line.split(',')
            # Get first 3 tokens, each of the dice.
            row = dict(zip(featureSet,len(featureSet)*[0]))
            for die in tokens[:3]:
                row[die] = 1

            row['Win Rate'] = float(tokens[-1])
            results.loc[len(results.index)] = row
    return results

if __name__ == "__main__":
    featureSet = getAllDice('dice.csv')
    data = loadData('results.csv',featureSet)
    resultScores = pd.DataFrame(columns = ['Linear', 'Ridge', 'SGD','SVM','MLP'])

    # Run each model through 10 iterations.
    for i in range(0,10):
        # Shuffle the dataset
        data = data.sample(frac = 1)

        # Use 10% of data for tests.
        trainingData = data.loc[:3654, data.columns != 'Win Rate']
        trainingLabels = data.loc[:3654, data.columns == 'Win Rate']
        testData = data.loc[3654:, data.columns != 'Win Rate']
        testLabels = data.loc[3654:, data.columns == 'Win Rate']

        model = linear_model.RidgeCV()
        model.fit(trainingData, trainingLabels)
        score = model.score(testData,testLabels)
        print("RidgeCV score: ", score)

        model2 = linear_model.SGDRegressor()
        model2.fit(trainingData, trainingLabels.values.ravel())
        score2 = model2.score(testData, testLabels)
        print("SGDRegressor score: ", score2)

        model3 = MLPRegressor(solver='lbfgs')
        model3.fit(trainingData, trainingLabels.values.ravel())
        score3 = model3.score(testData, testLabels)
        print("MLP Regressor: ", score3) 

        model4 = svm.SVR()
        model4.fit(trainingData, trainingLabels.values.ravel())
        score4 = model4.score(testData, testLabels)
        print("Support Vector Regression: ", score4)

        model5 = linear_model.LinearRegression()
        model5.fit(trainingData, trainingLabels.values.ravel())
        score5 = model5.score(testData, testLabels)
        print("Linear Regression score: ", score5)
        
        scores = {'Linear':score5, 'Ridge':score, 'SGD':score2, 'SVM':score4, 'MLP':score3}
        resultScores.loc[i] = scores

    print(resultScores)

    # Evaluate each feature in ridge model. Used to get the score for a "single" die.
    toPredict = pd.DataFrame(columns = list(featureSet))
    for row in featureSet:
        r = dict(zip(featureSet, len(featureSet)*[0]))
        r[row] = 1
        toPredict.loc[len(toPredict.index)] = r
    
    toPredict['Predicted Win Rate'] = model.predict(toPredict)
    
