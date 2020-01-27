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
-N-Fold Cross-Validated Sharpe Ratio
-N-Fold Cross-Validated Compounding Annual Return
-Nested Cross-Validated Sharpe Ratio
-Any Lean algorithm statistic (Sharpe Ratio, Alpha, Win Rate, etc)

Now also provided are several Blazor interfaces:

-Optimization Config Editor
-Optimization Results Charting
-Algorithm Code Editor (C#, Javascript)

WIP:
-Currently supporting browser threaded optimization (ie server-less) of basic algorithms
-Python running in browser
-User supplied C# to wasm compile in browser
-Genetic in browser

