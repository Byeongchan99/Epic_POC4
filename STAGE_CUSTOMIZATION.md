# 스테이지 커스터마이징 가이드

## 🎮 개요

Inspector에서 **각 스테이지의 패턴을 자유롭게 설정**할 수 있습니다!

## 📝 설정 방법

### 1. GameManager 오브젝트 선택
- Hierarchy에서 **GameManager** 선택
- Inspector 창 확인

### 2. Stage Settings 섹션 찾기
```
Inspector > GameManager
└── Stage Settings
    └── Stage Patterns (Array)
        ├── Size: 12 (스테이지 개수)
        ├── Element 0: Circle
        ├── Element 1: Triangle
        ├── Element 2: Square
        └── ...
```

### 3. 패턴 변경하기
각 Element의 드롭다운을 클릭하여 원하는 패턴 선택:

**사용 가능한 패턴:**
- **Circle** - 원형 (32개 선분)
- **Triangle** - 삼각형 (3개 선분)
- **Square** - 사각형 (4개 선분)
- **Pentagram** - 오각별 (5개 선분)
- **Hexagram** - 육각별 (13개 선분)
- **Heptagram** - 칠각별 (7개 선분)
- **Octagram** - 팔각별 (8개 선분)
- **DoublePentagram** - 이중 오각별 (11개 선분)
- **CrossPattern** - 십자가 패턴 (44개 선분)
- **Spiral** - 나선형 (64개 선분)
- **InfinitySymbol** - 무한대 기호 (64개 선분)
- **ComplexRune** - 복잡한 룬 (다양한 패턴 조합)

## 🎯 예시 설정

### 기본 설정 (현재)
```
Stage 1: Circle
Stage 2: Triangle
Stage 3: Square
Stage 4: Pentagram
Stage 5: Hexagram
Stage 6: Heptagram
Stage 7: Octagram
Stage 8: DoublePentagram
Stage 9: CrossPattern
Stage 10: Spiral
Stage 11: InfinitySymbol
Stage 12: ComplexRune
```

### 난이도 점진적 증가
```
Stage 1: Triangle (쉬움 - 선분 3개)
Stage 2: Square (쉬움 - 선분 4개)
Stage 3: Pentagram (보통 - 선분 5개)
Stage 4: Hexagram (보통 - 선분 13개)
Stage 5: Heptagram (보통 - 선분 7개)
Stage 6: Octagram (보통 - 선분 8개)
Stage 7: DoublePentagram (어려움 - 선분 11개)
Stage 8: Circle (어려움 - 선분 32개)
Stage 9: CrossPattern (매우 어려움 - 선분 44개)
Stage 10: Spiral (매우 어려움 - 선분 64개)
Stage 11: InfinitySymbol (매우 어려움 - 선분 64개)
Stage 12: ComplexRune (최고 난이도)
```

### 테마별 배치
```
Stage 1-3: 기본 도형 (Circle, Triangle, Square)
Stage 4-6: 별 모양 (Pentagram, Hexagram, Heptagram)
Stage 7-9: 복잡한 별 (Octagram, DoublePentagram, CrossPattern)
Stage 10-12: 특수 패턴 (Spiral, InfinitySymbol, ComplexRune)
```

### 같은 패턴 반복 (특정 패턴 연습)
```
Stage 1-4: Pentagram (오각별 마스터)
Stage 5-8: Spiral (나선 마스터)
Stage 9-12: ComplexRune (최종 보스)
```

## 🔧 스테이지 개수 변경하기

### 더 많은 스테이지 (예: 20개)
1. Inspector > GameManager > Stage Settings
2. **Size** 값을 `20`으로 변경
3. Element 12~19에 원하는 패턴 할당

### 더 적은 스테이지 (예: 5개)
1. Inspector > GameManager > Stage Settings
2. **Size** 값을 `5`로 변경
3. Element 0~4만 남음 (원하는 패턴 할당)

**자동 반영:**
- 배열 크기에 따라 전체 스테이지 개수 자동 조정
- UI에 "Stage X/Y" 형식으로 자동 표시

## 💡 난이도 조절 팁

### 쉽게 만들기
1. **적은 선분 패턴 사용**: Triangle, Square, Pentagram
2. **Cast Time 증가**: GameManager > Game Settings > Cast Time을 `5초`로
3. **Weakpoint Ratio 감소**: MagicCircle Prefab > Weakpoint Ratio를 `0.1`로

### 어렵게 만들기
1. **많은 선분 패턴 사용**: Circle, Spiral, InfinitySymbol, CrossPattern
2. **Cast Time 감소**: GameManager > Game Settings > Cast Time을 `2초`로
3. **Weakpoint Ratio 증가**: MagicCircle Prefab > Weakpoint Ratio를 `0.5`로

### 점진적 난이도 상승
- 초반 스테이지: 적은 선분 + 긴 시간 + 적은 약점
- 중반 스테이지: 중간 선분 + 보통 시간 + 보통 약점
- 후반 스테이지: 많은 선분 + 짧은 시간 + 많은 약점

## 🎨 패턴 특성

### 초보자 친화적
- **Triangle** (3개): 가장 쉬움
- **Square** (4개): 매우 쉬움
- **Pentagram** (5개): 쉬움

### 중급자용
- **Hexagram** (13개): 보통
- **Heptagram** (7개): 보통
- **Octagram** (8개): 보통
- **DoublePentagram** (11개): 보통-어려움

### 고급자용
- **Circle** (32개): 어려움 (많은 선분)
- **Spiral** (64개): 매우 어려움 (밀집된 선분)
- **InfinitySymbol** (64개): 매우 어려움 (복잡한 형태)
- **CrossPattern** (44개): 매우 어려움 (여러 방향)
- **ComplexRune**: 최고 난이도 (모든 요소 결합)

## 🚀 실전 예시

### 빠른 테스트 모드 (3스테이지)
```
Size: 3
Stage 1: Triangle
Stage 2: Pentagram
Stage 3: Circle
```

### 튜토리얼 모드 (5스테이지)
```
Size: 5
Stage 1: Triangle (기본 조작 배우기)
Stage 2: Square (감 잡기)
Stage 3: Pentagram (약점 찾기 연습)
Stage 4: Hexagram (복잡한 패턴 체험)
Stage 5: DoublePentagram (최종 시험)
```

### 엔드리스 모드 느낌 (30스테이지)
```
Size: 30
Stage 1-5: 기본 패턴 반복
Stage 6-15: 중급 패턴 믹스
Stage 16-30: 고급 패턴 랜덤 배치
```

## 📋 체크리스트

변경 후 확인사항:
- [ ] Inspector에서 패턴 배열이 제대로 표시되는가?
- [ ] Play 모드에서 설정한 패턴대로 나오는가?
- [ ] 스테이지 개수가 UI에 정확히 표시되는가?
- [ ] 마지막 스테이지 클리어 시 축하 화면이 나오는가?

## 🔄 기본값으로 되돌리기

Inspector에서 GameManager 컴포넌트 우클릭 > **Reset**
(주의: 다른 설정도 초기화됨!)

또는 수동으로:
```
Size: 12
Element 0: Circle
Element 1: Triangle
Element 2: Square
Element 3: Pentagram
Element 4: Hexagram
Element 5: Heptagram
Element 6: Octagram
Element 7: DoublePentagram
Element 8: CrossPattern
Element 9: Spiral
Element 10: InfinitySymbol
Element 11: ComplexRune
```

---

이제 스테이지를 **완전히 자유롭게** 커스터마이징할 수 있습니다! 🎉
