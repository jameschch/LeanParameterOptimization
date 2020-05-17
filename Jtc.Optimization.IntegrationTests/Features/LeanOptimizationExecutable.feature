Feature: LeanOptimizationExecutable

Scenario Outline: Optimize parameters
	Given I have an optimization.config
	And I have set maxThreads and generations to <maxThreads>
	And I have set useSharedAppDomain to <useSharedAppDomain>
	And the algorithm <isPython>
	And I have saved the test config
	When I run the Launcher executable
	Then the Sharpe Ratio on optimizer.txt should be <sharpRatio>
	#And Total Trades should be <totalTrades>
	#And last run should produce different result
	#And multiple threads should execute in parallel

	Examples:
		| maxThreads | useSharedAppDomain | isPython | sharpRatio | totalTrades |
		| 1          | true               | false    | 31.979     | 7           |
