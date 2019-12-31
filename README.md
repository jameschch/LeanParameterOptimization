# LeanParameterOptimization

Provides optimization of trading algorithm parameters for http://github.com/QuantConnect/Lean.

This is a WIP .net core/standard port of http://github.com/jameschch/LeanOptimization.

The optimizers support multiple parallel executions as standard. Currently, the following methods are available:
-Genetic Tournament
-Random Search
-Grid Search
-Particle Swarm
-Bayesian
-Globalized Bounded Nelder Mead

These methods can target several fitness and maximization goals:
-N-Fold Cross Validated Sharpe Ratio
-N-Fold Cross Validated Compounding Annual Return
-Nested Cross Validated Sharpe
-Any Lean algorithm statistic (Sharpe Ratio, Alpha, Win Rate etc)

Now also provided is a Blazor interface that allows editing of optimization configuration files as well as charting of optimization results.